using Bit.Core;
using Bit.Core.Utilities;
using Bit.Core.Enums;
using Bit.Core.Identity;
using Bit.Core.Models.Api;
using Bit.Core.Models.Table;
using Bit.Core.Repositories;
using Bit.Core.Services;
using Bit.Identity.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

// NOTE: directly support oidc matching email 
namespace Bit.Identity.Controllers
{

    public class AccountController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<AccountController> _logger;
        //private readonly ISsoConfigRepository _ssoConfigRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserService _userService;
        readonly GlobalSettings _globalSettings;

        public AccountController(
            IClientStore clientStore,
            IIdentityServerInteractionService interaction,
            ILogger<AccountController> logger,
            //ISsoConfigRepository ssoConfigRepository,
            IUserRepository userRepository,
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IHttpClientFactory clientFactory,
            IUserService userService,
            GlobalSettings globalSettings
            )
        {
            _clientStore = clientStore;
            _interaction = interaction;
            _logger = logger;
            //_ssoConfigRepository = ssoConfigRepository;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _clientFactory = clientFactory;
            _userService = userService;
            _globalSettings = globalSettings;
        }

        [HttpGet]
        public async Task<IActionResult> PreValidate(string domainHint)
        {

            // Everything is good!
            return new EmptyResult();

            if (string.IsNullOrWhiteSpace(domainHint))
            {
                Response.StatusCode = 400;
                return Json(new ErrorResponseModel("No domain hint was provided"));
            }
            try
            {
                // Calls Sso Pre-Validate, assumes baseUri set
                var requestCultureFeature = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var culture = requestCultureFeature.RequestCulture.Culture.Name;
                var requestPath = $"/Account/PreValidate?domainHint={domainHint}&culture={culture}";
                var httpClient = _clientFactory.CreateClient("InternalSso");
                using var responseMessage = await httpClient.GetAsync(requestPath);
                if (responseMessage.IsSuccessStatusCode)
                {
                    // All is good!
                    return new EmptyResult();
                }
                Response.StatusCode = (int)responseMessage.StatusCode;
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                return Content(responseJson, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pre-validating against SSO service");
                Response.StatusCode = 500;
                return Json(new ErrorResponseModel("Error pre-validating SSO authentication")
                {
                    ExceptionMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                    InnerExceptionMessage = ex.InnerException?.Message,
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            var domainHint = context.Parameters.AllKeys.Contains("domain_hint") ?
                context.Parameters["domain_hint"] : null;

            if (string.IsNullOrWhiteSpace(domainHint))
            {
                throw new Exception("No domain_hint provided");
            }

            var userIdentifier = context.Parameters.AllKeys.Contains("user_identifier") ?
                context.Parameters["user_identifier"] : null;

            return RedirectToAction(nameof(ExternalChallenge), new
            {
                organizationIdentifier = domainHint,
                returnUrl,
                userIdentifier
            });
        }

        [HttpGet]
        public async Task<IActionResult> ExternalChallenge(string organizationIdentifier, string returnUrl,
            string userIdentifier)
        {
            if (string.IsNullOrWhiteSpace(organizationIdentifier))
            {
                throw new Exception("Invalid organization reference id.");
            }
            // cherry always allow sso
            /*
                        var ssoConfig = await _ssoConfigRepository.GetByIdentifierAsync(organizationIdentifier);
                        if (ssoConfig == null || !ssoConfig.Enabled)
                        {
                            throw new Exception("Organization not found or SSO configuration not enabled");
                        }
                        var domainHint = ssoConfig.OrganizationId.ToString();
            */
            // well this is dummy to prevent any side effect if this is not exist..
            var domainHint = organizationIdentifier;

            var scheme = "sso";
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalCallback)),
                Items =
                {
                    { "return_url", returnUrl },
                    { "domain_hint", domainHint },
                    { "scheme", scheme },
                },
            };

            if (!string.IsNullOrWhiteSpace(userIdentifier))
            {
                props.Items.Add("user_identifier", userIdentifier);
            }

            return Challenge(props, scheme);
        }

        [HttpGet]
        public async Task<ActionResult> ExternalCallback()
        {
            // Read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(
                Core.AuthenticationSchemes.BitwardenExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }
            
            var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);

            var user = await FindUserFromExternalProviderAsync(result.Principal);
            if (user == null)
            {
                // cherry   2021/02/03
                user = await AutoProvisionUserAsync(result.Principal);
            }

            // This allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1)
            };
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);
            var provider = result.Properties.Items["scheme"];
            // Issue authentication cookie for user
            await HttpContext.SignInAsync(new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = user.Email,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims.ToArray()
            }, localSignInProps);

            // Delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(Core.AuthenticationSchemes.BitwardenExternalCookieAuthenticationScheme);

            // Retrieve return URL
            var returnUrl = result.Properties.Items["return_url"] ?? "~/";

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                if (IsNativeClient(context))
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.Headers["Location"] = string.Empty;
                    return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
                }

                // We can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl);
            }

            // Request for a local page
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                // User might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }

        // not using any sso tables just use email as the external id        
        private async Task<User> FindUserFromExternalProviderAsync(ClaimsPrincipal externalUser)
        {
/*
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");
*/
            var emailClaim = externalUser.FindFirst(JwtClaimTypes.Email) ??
                              throw new Exception("email claim not found");
            // TODO fix this.. merged sso into here. use email as identifier
            //var user = await _userRepository.GetBySsoUserAsync(providerUserId, config.OrganizationId);
            var user = await _userRepository.GetByEmailAsync(emailClaim.Value);
            //var user = await _userRepository.GetByIdAsync(new Guid(providerUserId));            

            return user;
        }

        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // If the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // If the external provider issued an idToken, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(
                    new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }

        public bool IsNativeClient(IdentityServer4.Models.AuthorizationRequest context)
        {
            return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
               && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        private async Task<User> AutoProvisionUserAsync(ClaimsPrincipal claims)
        {
            var name = claims.GetName();
            var email = claims.GetEmailAddress();
            var orgIdentifier = claims.GetClaim(_globalSettings.Sso.OrganizationIdentifierClaimType);

            OrganizationUser orgUser = null;
            var organization = await _organizationRepository.GetByIdentifierAsync(orgIdentifier);
            if (organization == null)
                throw new Exception($"Organization: {orgIdentifier} not found" );

            // Create user record - all existing user flows are handled above
            var user = new User
            {
                Name = name,
                Email = email,
                EmailVerified = true,
                ApiKey = CoreHelpers.SecureRandomString(30)
            };
            await _userService.RegisterUserAsync(user);


            orgUser = new OrganizationUser
            {
                OrganizationId = organization.Id,
                UserId = user.Id,
                Type = OrganizationUserType.User,
                Status = OrganizationUserStatusType.Invited
            };
            await _organizationUserRepository.CreateAsync(orgUser);

            return user;
        }
    }
}
