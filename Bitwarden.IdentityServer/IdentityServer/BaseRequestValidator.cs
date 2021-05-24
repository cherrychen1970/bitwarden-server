﻿using Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Repositories;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bit.Core.Services;
using System.Linq;
using Bit.Core.Models;
using Bit.Core.Identity;
using Bit.Core.Models.Data;
using Bit.Core.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Bit.Core.Models.Api;

namespace Bit.Core.IdentityServer
{
    public abstract class BaseRequestValidator<T> where T : class
    {
        private UserManager<User> _userManager;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;        
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IApplicationCacheService _applicationCacheService;
        private readonly IMailService _mailService;
        private readonly ILogger<ResourceOwnerPasswordValidator> _logger;
        //private readonly ISessionContext _currentContext;
        private readonly GlobalSettings _globalSettings;
        private readonly IPolicyRepository _policyRepository;

        private readonly HttpContext _httpContext;

        public BaseRequestValidator(
            UserManager<User> userManager,
            IDeviceRepository deviceRepository,
            IDeviceService deviceService,
            IUserService userService,
            IEventService eventService,            
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IApplicationCacheService applicationCacheService,
            IMailService mailService,
            ILogger<ResourceOwnerPasswordValidator> logger,
            IHttpContextAccessor httpContextAccessor,
            GlobalSettings globalSettings,
            IPolicyRepository policyRepository)
        {
            _userManager = userManager;
            _deviceRepository = deviceRepository;
            _deviceService = deviceService;
            _userService = userService;
            _eventService = eventService;            
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _applicationCacheService = applicationCacheService;
            _mailService = mailService;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
            _globalSettings = globalSettings;
            _policyRepository = policyRepository;
        }

        protected async Task ValidateAsync(T context, ValidatedTokenRequest request)
        {
            var twoFactorToken = request.Raw["TwoFactorToken"]?.ToString();
            var twoFactorProvider = request.Raw["TwoFactorProvider"]?.ToString();
            var twoFactorRemember = request.Raw["TwoFactorRemember"]?.ToString() == "1";
            var twoFactorRequest = !string.IsNullOrWhiteSpace(twoFactorToken) &&
                !string.IsNullOrWhiteSpace(twoFactorProvider);

            var (user, valid) = await ValidateContextAsync(context);
            if (!valid)
            {
                await BuildErrorResultAsync("Username or password is incorrect. Try again.", false, context, user);
                return;
            }

            twoFactorRequest = false;
            twoFactorRemember = false;
            twoFactorToken = null;

            // Returns true if can finish validation process
            if (await IsValidAuthTypeAsync(user, request.GrantType))
            {
                var device = await SaveDeviceAsync(user, request);
                if (device == null)
                {
                    await BuildErrorResultAsync("No device information provided.", false, context, user);
                    return;
                }
                await BuildSuccessResultAsync(user, context, device, twoFactorRequest && twoFactorRemember);
            }
            else
            {
                SetSsoResult(context, new Dictionary<string, object>
                {{
                    "ErrorModel", new ErrorResponseModel("SSO authentication is required.")
                }});
            }
        }

        protected abstract Task<(User, bool)> ValidateContextAsync(T context);

        protected async Task BuildSuccessResultAsync(User user, T context, Device device, bool sendRememberToken)
        {
            await _eventService.LogUserEventAsync(user.Id, EventType.User_LoggedIn);

            var claims = new List<Claim>();

            if (device != null)
            {
                claims.Add(new Claim("device", device.Identifier));
            }

            var customResponse = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(user.PrivateKey))
            {
                customResponse.Add("PrivateKey", user.PrivateKey);
            }

            if (!string.IsNullOrWhiteSpace(user.Key))
            {
                customResponse.Add("Key", user.Key);
            }

            customResponse.Add("ResetMasterPassword", string.IsNullOrWhiteSpace(user.MasterPassword));
            customResponse.Add("Kdf", (byte)user.Kdf);
            customResponse.Add("KdfIterations", user.KdfIterations);

            if (sendRememberToken)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user,
                    CoreHelpers.CustomProviderName(TwoFactorProviderType.Remember));
                customResponse.Add("TwoFactorToken", token);
            }

            SetSuccessResult(context, user, claims, customResponse);
        }

        protected async Task BuildErrorResultAsync(string message, bool twoFactorRequest, T context, User user)
        {
            if (user != null)
            {
                await _eventService.LogUserEventAsync(user.Id,
                    twoFactorRequest ? EventType.User_FailedLogIn2fa : EventType.User_FailedLogIn);
            }

            if (_globalSettings.SelfHosted)
            {
                _logger.LogWarning(Constants.BypassFiltersEventId,
                    string.Format("Failed login attempt{0}{1}", twoFactorRequest ? ", 2FA invalid." : ".",
                        $" {_httpContext.GetIpAddress()}"));
            }

            await Task.Delay(2000); // Delay for brute force.
            SetErrorResult(context,
                new Dictionary<string, object>
                {{
                    "ErrorModel", new ErrorResponseModel(message)
                }});
        }

        protected abstract void SetTwoFactorResult(T context, Dictionary<string, object> customResponse);

        protected abstract void SetSsoResult(T context, Dictionary<string, object> customResponse);

        protected abstract void SetSuccessResult(T context, User user, List<Claim> claims,
            Dictionary<string, object> customResponse);

        protected abstract void SetErrorResult(T context, Dictionary<string, object> customResponse);

        private async Task<Tuple<bool, Organization>> RequiresTwoFactorAsync(User user)
        {
            var individualRequired = _userManager.SupportsUserTwoFactor &&
                await _userManager.GetTwoFactorEnabledAsync(user) &&
                (await _userManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

            Organization firstEnabledOrg = null;
            var orgs = await _organizationUserRepository.GetMemberships(user.Id);

            return new Tuple<bool, Organization>(individualRequired || firstEnabledOrg != null, firstEnabledOrg);
        }

        private async Task<bool> IsValidAuthTypeAsync(User user, string grantType)
        {
            if (grantType == "authorization_code")
            {
                // Already using SSO to authorize, finish successfully
                return true;
            }
            // Default - continue validation process
            return true;
        }

        private bool OrgUsing2fa(IDictionary<Guid, OrganizationAbility> orgAbilities, Guid orgId)
        {
            return orgAbilities != null && orgAbilities.ContainsKey(orgId) &&
                orgAbilities[orgId].Enabled && orgAbilities[orgId].Using2fa;
        }

        private bool OrgCanUseSso(IDictionary<Guid, OrganizationAbility> orgAbilities, Guid orgId)
        {
            return orgAbilities != null && orgAbilities.ContainsKey(orgId) &&
                   orgAbilities[orgId].Enabled && orgAbilities[orgId].UseSso;
        }

        private Device GetDeviceFromRequest(ValidatedRequest request)
        {
            var deviceIdentifier = request.Raw["DeviceIdentifier"]?.ToString();
            var deviceType = request.Raw["DeviceType"]?.ToString();
            var deviceName = request.Raw["DeviceName"]?.ToString();
            var devicePushToken = request.Raw["DevicePushToken"]?.ToString();

            if (string.IsNullOrWhiteSpace(deviceIdentifier) || string.IsNullOrWhiteSpace(deviceType) ||
                string.IsNullOrWhiteSpace(deviceName) || !Enum.TryParse(deviceType, out DeviceType type))
            {
                return null;
            }

            return new Device
            {
                Identifier = deviceIdentifier,
                Name = deviceName,
                Type = type,
                PushToken = string.IsNullOrWhiteSpace(devicePushToken) ? null : devicePushToken
            };
        }
        private async Task<Device> SaveDeviceAsync(User user, ValidatedTokenRequest request)
        {
            var device = GetDeviceFromRequest(request);
            if (device != null)
            {
                var existingDevice = await _deviceRepository.GetByIdentifierAsync(device.Identifier, user.Id);
                if (existingDevice == null)
                {
                    device.UserId = user.Id;
                    await _deviceService.SaveAsync(device);

                    var now = DateTime.UtcNow;
                    if (now - user.CreationDate > TimeSpan.FromMinutes(10))
                    {
                        var deviceType = device.Type.GetType().GetMember(device.Type.ToString())
                            .FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName();
                        if (!_globalSettings.DisableEmailNewDevice)
                        {
                            await _mailService.SendNewDeviceLoggedInEmail(user.Email, deviceType, now,
                                _httpContext.GetIpAddress());
                        }
                    }

                    return device;
                }

                return existingDevice;
            }

            return null;
        }
    }
}
