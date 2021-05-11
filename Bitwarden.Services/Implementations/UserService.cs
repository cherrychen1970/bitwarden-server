using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bit.Core.Models;
using Bit.Core.Repositories;
using System.Linq;
using Bit.Core.Enums;
using System.Security.Claims;
using Bit.Core.Models;
using Bit.Core.Models.Business;
using U2fLib = U2F.Core.Crypto.U2F;
using U2F.Core.Models;
using U2F.Core.Utils;
using Bit.Core.Exceptions;
using Bit.Core.Utilities;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.DataProtection;
using U2F.Core.Exceptions;

namespace Bit.Core.Services
{
    public class UserService : UserManager<User>, IUserService, IDisposable
    //public class UserService : IUserService, IDisposable
    {    
        private readonly IUserRepository _userRepository;
        private readonly ICipherRepository _cipherRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMailService _mailService;
        private readonly IPushNotificationService _pushService;
        private readonly IdentityErrorDescriber _identityErrorDescriber;
        private readonly IdentityOptions _identityOptions;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;        
        private readonly IEventService _eventService;
        private readonly IApplicationCacheService _applicationCacheService;
        private readonly IPolicyRepository _policyRepository;
        private readonly IDataProtector _organizationServiceDataProtector;
        private readonly IReferenceEventService _referenceEventService;
        //private readonly ISessionContext _currentContext;
        private readonly GlobalSettings _globalSettings;
        private readonly IOrganizationService _organizationService;
        private readonly HttpContext _httpContext;

        public UserService(
            IUserRepository userRepository,
            ICipherRepository cipherRepository,
            IOrganizationUserRepository organizationUserRepository,
            IOrganizationRepository organizationRepository,            
            IMailService mailService,
            IPushNotificationService pushService,
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger,            
            IEventService eventService,
            IApplicationCacheService applicationCacheService,
            IDataProtectionProvider dataProtectionProvider,            
            IPolicyRepository policyRepository,
            IReferenceEventService referenceEventService,
            //ISessionContext currentContext,
            IHttpContextAccessor httpContextAccessor,
            GlobalSettings globalSettings,
            IOrganizationService organizationService)
            : base(
                  store,
                  optionsAccessor,
                  passwordHasher,
                  userValidators,
                  passwordValidators,
                  keyNormalizer,
                  errors,
                  services,
                  logger)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userRepository = userRepository;
            _cipherRepository = cipherRepository;
            _organizationUserRepository = organizationUserRepository;
            _organizationRepository = organizationRepository;            
            _mailService = mailService;
            _pushService = pushService;
            _identityOptions = optionsAccessor?.Value ?? new IdentityOptions();
            _identityErrorDescriber = errors;
            _passwordHasher = passwordHasher;
            _passwordValidators = passwordValidators;            
            _eventService = eventService;
            _applicationCacheService = applicationCacheService;            
            _policyRepository = policyRepository;
            _organizationServiceDataProtector = dataProtectionProvider.CreateProtector(
                "OrganizationServiceDataProtector");
            _referenceEventService = referenceEventService;
            //_currentContext = currentContext;
            _globalSettings = globalSettings;
            _organizationService = organizationService;
        }



        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);            
        }
        public async Task<DateTime> GetAccountRevisionDateByIdAsync(Guid userId)
        {
            return await _userRepository.GetAccountRevisionDateAsync(userId);
        }

        public async Task SaveUserAsync(User user, bool push = false)
        {
            if (user.Id == default(Guid))
            {
                throw new ApplicationException("Use register method to create a new user.");
            }
            
            await _userRepository.ReplaceAsync(user);

            if (push)
            {
                // push
                await _pushService.PushSyncSettingsAsync(user.Id);
            }
        }

        public override async Task<IdentityResult> DeleteAsync(User user)
        {
            /*
            // Check if user is the only owner of any organizations.
            var onlyOwnerCount = await _organizationUserRepository.GetCountAsync(x=>x.UserId==user.Id && x.Type==OrganizationUserType.Owner);
            if (onlyOwnerCount > 0)
            {
                var deletedOrg = false;
                var orgs = await _organizationUserRepository.GetManyDetailsByUserAsync(user.Id,
                    OrganizationUserStatusType.Confirmed);
                if (orgs.Count == 1)
                {
                    var org = await _organizationRepository.GetByIdAsync(orgs.First().OrganizationId);
                    if (org != null && (!org.Enabled || string.IsNullOrWhiteSpace(org.GatewaySubscriptionId)))
                    {
                        var orgCount = await _organizationUserRepository.GetCountByOrganizationIdAsync(org.Id);
                        if (orgCount <= 1)
                        {
                            await _organizationRepository.DeleteAsync(org);
                            deletedOrg = true;
                        }
                    }
                }

                if (!deletedOrg)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "You must leave or delete any organizations that you are the only owner of first."
                    });
                }
            }

            await _userRepository.DeleteAsync(user);
            await _referenceEventService.RaiseEventAsync(
                new ReferenceEvent(ReferenceEventType.DeleteAccount, user));
            await _pushService.PushLogOutAsync(user.Id);
            return IdentityResult.Success;
            */
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteAsync(User user, string token)
        {
            if (!(await VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "DeleteAccount", token)))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            return await DeleteAsync(user);
        }

        public async Task SendDeleteConfirmationAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // No user exists.
                return;
            }

            var token = await base.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "DeleteAccount");
            await _mailService.SendVerifyDeleteEmailAsync(user.Email, user.Id, token);
        }

        public async Task<IdentityResult> RegisterUserAsync(User user, string masterPassword,
            string token, Guid? orgUserId)
        {
            var tokenValid = false;
            if (_globalSettings.DisableUserRegistration && !string.IsNullOrWhiteSpace(token) && orgUserId.HasValue)
            {
                tokenValid = CoreHelpers.UserInviteTokenIsValid(_organizationServiceDataProtector, token,
                    user.Email, orgUserId.Value, _globalSettings);
            }

            if (_globalSettings.DisableUserRegistration && !tokenValid)
            {
                throw new BadRequestException("Open registration has been disabled by the system administrator.");
            }
            user.ApiKey = CoreHelpers.SecureRandomString(30);
            var result = await base.CreateAsync(user, masterPassword);
            if (result == IdentityResult.Success)
            {
                await _mailService.SendWelcomeEmailAsync(user);
                await _referenceEventService.RaiseEventAsync(new ReferenceEvent(ReferenceEventType.Signup, user));
            }

            return result;
        }

        public async Task<IdentityResult> RegisterUserAsync(User user)
        {
            var result = await base.CreateAsync(user);
            if (result == IdentityResult.Success)
            {
                await _mailService.SendWelcomeEmailAsync(user);
                await _referenceEventService.RaiseEventAsync(new ReferenceEvent(ReferenceEventType.Signup, user));
            }

            return result;
        }

        public async Task SendMasterPasswordHintAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // No user exists. Do we want to send an email telling them this in the future?
                return;
            }

            if (string.IsNullOrWhiteSpace(user.MasterPasswordHint))
            {
                await _mailService.SendNoMasterPasswordHintEmailAsync(email);
                return;
            }

            await _mailService.SendMasterPasswordHintEmailAsync(email, user.MasterPasswordHint);
        }

        public async Task SendTwoFactorEmailAsync(User user)
        {
            var provider = user.GetTwoFactorProvider(TwoFactorProviderType.Email);
            if (provider == null || provider.MetaData == null || !provider.MetaData.ContainsKey("Email"))
            {
                throw new ArgumentNullException("No email.");
            }

            var email = ((string)provider.MetaData["Email"]).ToLowerInvariant();
            var token = await base.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider,
                "2faEmail:" + email);
            await _mailService.SendTwoFactorEmailAsync(email, token);
        }

        public async Task<bool> VerifyTwoFactorEmailAsync(User user, string token)
        {
            var provider = user.GetTwoFactorProvider(TwoFactorProviderType.Email);
            if (provider == null || provider.MetaData == null || !provider.MetaData.ContainsKey("Email"))
            {
                throw new ArgumentNullException("No email.");
            }

            var email = ((string)provider.MetaData["Email"]).ToLowerInvariant();
            return await base.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider,
                "2faEmail:" + email, token);
        }

        public async Task SendEmailVerificationAsync(User user)
        {
            if (user.EmailVerified)
            {
                throw new BadRequestException("Email already verified.");
            }

            var token = await base.GenerateEmailConfirmationTokenAsync(user);
            await _mailService.SendVerifyEmailEmailAsync(user.Email, user.Id, token);
        }

        public async Task InitiateEmailChangeAsync(User user, string newEmail)
        {
            var existingUser = await _userRepository.GetByEmailAsync(newEmail);
            if (existingUser != null)
            {
                await _mailService.SendChangeEmailAlreadyExistsEmailAsync(user.Email, newEmail);
                return;
            }

            var token = await base.GenerateChangeEmailTokenAsync(user, newEmail);
            await _mailService.SendChangeEmailEmailAsync(newEmail, token);
        }

        public async Task<IdentityResult> ChangeEmailAsync(User user, string masterPassword, string newEmail,
            string newMasterPassword, string token, string key)
        {
            var verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.MasterPassword, masterPassword);
            if (verifyPasswordResult == PasswordVerificationResult.Failed)
            {
                return IdentityResult.Failed(_identityErrorDescriber.PasswordMismatch());
            }

            if (!await base.VerifyUserTokenAsync(user, _identityOptions.Tokens.ChangeEmailTokenProvider,
                GetChangeEmailTokenPurpose(newEmail), token))
            {
                return IdentityResult.Failed(_identityErrorDescriber.InvalidToken());
            }

            var existingUser = await _userRepository.GetByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return IdentityResult.Failed(_identityErrorDescriber.DuplicateEmail(newEmail));
            }

            var result = await UpdatePasswordHash(user, newMasterPassword);
            if (!result.Succeeded)
            {
                return result;
            }

            user.Key = key;
            user.Email = newEmail;
            user.EmailVerified = true;
            user.AccountRevisionDate = DateTime.UtcNow;
            await _userRepository.ReplaceAsync(user);
            await _pushService.PushLogOutAsync(user.Id);

            return IdentityResult.Success;
        }

        public override Task<IdentityResult> ChangePasswordAsync(User user, string masterPassword, string newMasterPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string masterPassword, string newMasterPassword,
            string key)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (await CheckPasswordAsync(user, masterPassword))
            {
                var result = await UpdatePasswordHash(user, newMasterPassword);
                if (!result.Succeeded)
                {
                    return result;
                }
                
                user.Key = key;

                await _userRepository.ReplaceAsync(user);
                await _eventService.LogUserEventAsync(user.Id, EventType.User_ChangedPassword);
                await _pushService.PushLogOutAsync(user.Id);

                return IdentityResult.Success;
            }

            Logger.LogWarning("Change password failed for user {userId}.", user.Id);
            return IdentityResult.Failed(_identityErrorDescriber.PasswordMismatch());
        }

        public async Task<IdentityResult> SetPasswordAsync(User user, string masterPassword, string key)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!string.IsNullOrWhiteSpace(user.MasterPassword))
            {
                Logger.LogWarning("Change password failed for user {userId} - already has password.", user.Id);
                return IdentityResult.Failed(_identityErrorDescriber.UserAlreadyHasPassword());
            }

            var result = await UpdatePasswordHash(user, masterPassword, true, false);
            if (!result.Succeeded)
            {
                return result;
            }
            
            user.Key = key;

            await _userRepository.ReplaceAsync(user);
            await _eventService.LogUserEventAsync(user.Id, EventType.User_ChangedPassword);

            var orgs = await _organizationUserRepository.GetManyByUserAsync(user.Id);
            foreach (var orgUser in orgs)
            {
                if (orgUser.Status == OrganizationUserStatusType.Invited)
                {
                    await _organizationService.AcceptUserAsync(orgUser);
                }               
            }            
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangeKdfAsync(User user, string masterPassword, string newMasterPassword,
            string key, KdfType kdf, int kdfIterations)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (await CheckPasswordAsync(user, masterPassword))
            {
                var result = await UpdatePasswordHash(user, newMasterPassword);
                if (!result.Succeeded)
                {
                    return result;
                }
                
                user.Key = key;
                user.Kdf = kdf;
                user.KdfIterations = kdfIterations;
                await _userRepository.ReplaceAsync(user);
                await _pushService.PushLogOutAsync(user.Id);
                return IdentityResult.Success;
            }

            Logger.LogWarning("Change KDF failed for user {userId}.", user.Id);
            return IdentityResult.Failed(_identityErrorDescriber.PasswordMismatch());
        }

        public async Task<IdentityResult> UpdateKeyAsync(User user, string masterPassword, string key, string privateKey,
            IEnumerable<UserCipher> ciphers, IEnumerable<Folder> folders)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (await CheckPasswordAsync(user, masterPassword))
            {                
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.Key = key;
                user.PrivateKey = privateKey;
                if (ciphers.Any() || folders.Any())
                {
                    await _userRepository.ReplaceAsync(user);
                    await _cipherRepository.UpdateManyAsync(ciphers,user.Id);
                }
                else
                {
                    await _userRepository.ReplaceAsync(user);
                }

                await _pushService.PushLogOutAsync(user.Id);
                return IdentityResult.Success;
            }

            Logger.LogWarning("Update key failed for user {userId}.", user.Id);
            return IdentityResult.Failed(_identityErrorDescriber.PasswordMismatch());
        }

        public async Task<IdentityResult> RefreshSecurityStampAsync(User user, string masterPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (await CheckPasswordAsync(user, masterPassword))
            {
                var result = await base.UpdateSecurityStampAsync(user);
                if (!result.Succeeded)
                {
                    return result;
                }

                await SaveUserAsync(user);
                await _pushService.PushLogOutAsync(user.Id);
                return IdentityResult.Success;
            }

            Logger.LogWarning("Refresh security stamp failed for user {userId}.", user.Id);
            return IdentityResult.Failed(_identityErrorDescriber.PasswordMismatch());
        }
   
       public override async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user == null)
            {
                return false;
            }

            var result = await base.VerifyPasswordAsync(Store as IUserPasswordStore<User>, user, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await UpdatePasswordHash(user, password, false, false);                
                await _userRepository.ReplaceAsync(user);
            }

            var success = result != PasswordVerificationResult.Failed;
            if (!success)
            {
                Logger.LogWarning(0, "Invalid password for user {userId}.", user.Id);
            }
            return success;
        }

        public async Task<string> GenerateSignInTokenAsync(User user, string purpose)
        {
            var token = await GenerateUserTokenAsync(user, Options.Tokens.PasswordResetTokenProvider,
                purpose);
            return token;
        }
        
        private async Task<IdentityResult> UpdatePasswordHash(User user, string newPassword,
            bool validatePassword = true, bool refreshStamp = true)
        {
            if (validatePassword)
            {
                var validate = await ValidatePasswordInternal(user, newPassword);
                if (!validate.Succeeded)
                {
                    return validate;
                }
            }

            user.MasterPassword = _passwordHasher.HashPassword(user, newPassword);
            if (refreshStamp)
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            return IdentityResult.Success;
        }

        private async Task<IdentityResult> ValidatePasswordInternal(User user, string password)
        {
            var errors = new List<IdentityError>();
            foreach (var v in _passwordValidators)
            {
                var result = await v.ValidateAsync(this, user, password);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }

            if (errors.Count > 0)
            {
                Logger.LogWarning("User {userId} password validation failed: {errors}.", await GetUserIdAsync(user),
                    string.Join(";", errors.Select(e => e.Code)));
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            var result = await base.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _referenceEventService.RaiseEventAsync(
                    new ReferenceEvent(ReferenceEventType.ConfirmEmailAddress, user));
            }
            return result;
        }

        public async Task RotateApiKeyAsync(User user)
        {
            user.ApiKey = CoreHelpers.SecureRandomString(30);            
            await _userRepository.ReplaceAsync(user);
        }
    }
}
