using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Bit.Core;
using Bit.Core.Utilities;
using AspNetCoreRateLimit;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
//using Bit.Identity.Utilities;
using IdentityServer4.Extensions;
using IdentityServer4.AccessTokenValidation;
using IdentityModel;

namespace Bit.Api
{
    static public class StartupExtension
    {
        static public void AddCustomCookiePolicy(this IServiceCollection services)
        {
                services.Configure<CookiePolicyOptions>(options =>
                {
                    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                    options.OnAppendCookie = ctx =>
                    {
                        ctx.CookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                    };
                });
        }
        static public void AddCustomSingleSignOn(this IServiceCollection services, GlobalSettings globalSettings)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
           // Authentication
            services
                .AddDistributedIdentityServices(globalSettings)
                .AddAuthentication()
                .AddCookie(AuthenticationSchemes.BitwardenExternalCookieAuthenticationScheme)
                // TODO : it is not using internalSSo and direclty can go any oidc 
                // email is the identifier to match bitwarden user
                .AddOpenIdConnect("sso", "Single Sign On", options =>
                {
                    options.Authority = globalSettings.Oidc.Authority;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = globalSettings.Oidc.Client;
                    options.ClientSecret = globalSettings.Oidc.ClientSecret;
                    options.ResponseMode = "form_post";

                    options.SignInScheme = AuthenticationSchemes.BitwardenExternalCookieAuthenticationScheme;
                    options.ResponseType = "code";
                    options.SaveTokens = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    // well this is not automatic...                    
                    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Email,JwtClaimTypes.Email);                    
                    options.ClaimActions.MapUniqueJsonKey(globalSettings.Oidc.OrganizationIdentifier,globalSettings.Oidc.OrganizationIdentifier);                    
                    //foreach (var item in globalSettings.Oidc.Scopes)                    
                    //    options.Scope.Add(item);

                    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            //cherry. behind proxy
                            //context.ProtocolMessage.RedirectUri = $"{globalSettings.BaseServiceUri.Identity}/signin-oidc";
                            // Pass domain_hint onto the sso idp
                            context.ProtocolMessage.DomainHint = context.Properties.Items["domain_hint"];
                            if (context.Properties.Items.ContainsKey("user_identifier"))
                            {
                                context.ProtocolMessage.SessionState = context.Properties.Items["user_identifier"];
                            }
                            return Task.FromResult(0);
                        }                        
                    };                   
                });            
        }

        public static void AddCustomAuthorizationPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {               
                options.AddPolicy("Application", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer")
                        .RequireAuthenticatedUser()
                        .RequireClaim(JwtClaimTypes.AuthenticationMethod, "Application", "external")
                        .RequireClaim(JwtClaimTypes.Scope, "api");
                });
                options.AddPolicy("Web", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(JwtClaimTypes.AuthenticationMethod, "Application", "external");
                    policy.RequireClaim(JwtClaimTypes.Scope, "api");
                    policy.RequireClaim(JwtClaimTypes.ClientId, "web");
                });
                options.AddPolicy("Push", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(JwtClaimTypes.Scope, "api.push");
                });
                options.AddPolicy("Licensing", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(JwtClaimTypes.Scope, "api.licensing");
                });
                options.AddPolicy("Organization", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(JwtClaimTypes.Scope, "api.organization");
                });
            });            
        }        
    }
}
