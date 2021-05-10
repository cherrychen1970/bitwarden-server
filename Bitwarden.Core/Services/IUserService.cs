using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Bit.Core.Models;
using System.Security.Claims;
using Bit.Core.Enums;
using Bit.Core.Models;
using Bit.Core.Models.Business;

namespace Bit.Core.Services
{
    public interface IUserService
    {
        /*
        Guid? GetProperUserId(ClaimsPrincipal principal);
        Task<User> GetUserByIdAsync(string userId);
        
        Task<User> GetUserByPrincipalAsync(ClaimsPrincipal principal);
        */
        Task<User> GetUserByIdAsync(Guid userId);
        Task<DateTime> GetAccountRevisionDateByIdAsync(Guid userId);
        Task SaveUserAsync(User user, bool push = false);
        Task<IdentityResult> RegisterUserAsync(User user, string masterPassword, string token, Guid? orgUserId);
        Task<IdentityResult> RegisterUserAsync(User user);
        Task SendMasterPasswordHintAsync(string email);
        Task SendTwoFactorEmailAsync(User user);
        Task<bool> VerifyTwoFactorEmailAsync(User user, string token);
        Task<U2fRegistration> StartU2fRegistrationAsync(User user);
        Task<bool> DeleteU2fKeyAsync(User user, int id);
        Task<bool> CompleteU2fRegistrationAsync(User user, int id, string name, string deviceResponse);
        Task SendEmailVerificationAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task InitiateEmailChangeAsync(User user, string newEmail);
        Task<IdentityResult> ChangeEmailAsync(User user, string masterPassword, string newEmail, string newMasterPassword,
            string token, string key);
        Task<IdentityResult> ChangePasswordAsync(User user, string masterPassword, string newMasterPassword, string key);
        Task<IdentityResult> SetPasswordAsync(User user, string newMasterPassword, string key);
        Task<IdentityResult> ChangeKdfAsync(User user, string masterPassword, string newMasterPassword, string key,
            KdfType kdf, int kdfIterations);
        Task<IdentityResult> UpdateKeyAsync(User user, string masterPassword, string key, string privateKey,
            IEnumerable<UserCipher> ciphers, IEnumerable<Folder> folders);
        Task<IdentityResult> RefreshSecurityStampAsync(User user, string masterPasswordHash);
        Task UpdateTwoFactorProviderAsync(User user, TwoFactorProviderType type);
        Task DisableTwoFactorProviderAsync(User user, TwoFactorProviderType type,
            IOrganizationService organizationService);
        Task<bool> RecoverTwoFactorAsync(string email, string masterPassword, string recoveryCode,
            IOrganizationService organizationService);
        Task<string> GenerateUserTokenAsync(User user, string tokenProvider, string purpose);
        Task<IdentityResult> DeleteAsync(User user);
        Task<IdentityResult> DeleteAsync(User user, string token);
        Task SendDeleteConfirmationAsync(string email);
        Task IapCheckAsync(User user, PaymentMethodType paymentMethodType);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> CanAccessPremium(ITwoFactorProvidersUser user);
        Task<bool> TwoFactorIsEnabledAsync(ITwoFactorProvidersUser user);
        Task<bool> TwoFactorProviderIsEnabledAsync(TwoFactorProviderType provider, ITwoFactorProvidersUser user);
        Task<string> GenerateEnterprisePortalSignInTokenAsync(User user);
        Task<string> GenerateSignInTokenAsync(User user, string purpose);
        Task RotateApiKeyAsync(User user);
    }
}
