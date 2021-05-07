using System.Threading.Tasks;
using Bit.Core.Models.Business;
using Bit.Core.Models;
using System;
using System.Collections.Generic;
using Bit.Core.Enums;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    public interface IOrganizationService
    {
        Task CancelSubscriptionAsync(Guid organizationId, bool? endOfPeriod = null);
        Task ReinstateSubscriptionAsync(Guid organizationId);        
        Task<Tuple<Organization, OrganizationUser>> SignUpAsync(OrganizationSignup organizationSignup);
        Task DeleteAsync(Organization organization);
        Task EnableAsync(Guid organizationId, DateTime? expirationDate);
        Task DisableAsync(Guid organizationId, DateTime? expirationDate);        
        Task EnableAsync(Guid organizationId);
        Task UpdateAsync(Organization organization, bool updateBilling = false);
        Task UpdateTwoFactorProviderAsync(Organization organization, TwoFactorProviderType type);
        Task DisableTwoFactorProviderAsync(Organization organization, TwoFactorProviderType type);
        Task<List<OrganizationUser>> InviteUserAsync(Guid organizationId, Guid? invitingUserId, string externalId, OrganizationUserInvite orgUserInvite);
        Task ResendInviteAsync(Guid organizationId, Guid? invitingUserId, Guid organizationUserId);
        Task<OrganizationUser> AcceptUserAsync(Guid organizationUserId, User user, string token,
            IUserService userService);
        Task<OrganizationUser> AcceptUserAsync(Guid organizationId, User user, IUserService userService);
        Task<OrganizationUser> ConfirmUserAsync(Guid organizationId, Guid organizationUserId, string key,
            Guid confirmingUserId, IUserService userService);
        Task SaveUserAsync(OrganizationUser user, Guid? savingUserId, IEnumerable<SelectionReadOnly> collections);
        Task DeleteUserAsync(Guid organizationId, Guid organizationUserId, Guid? deletingUserId);
        Task DeleteUserAsync(Guid organizationId, Guid userId);
        Task UpdateUserGroupsAsync(OrganizationUser organizationUser, IEnumerable<Guid> groupIds, Guid? loggedInUserId);
        Task ImportAsync(Guid organizationId, Guid? importingUserId, IEnumerable<ImportedGroup> groups,
            IEnumerable<ImportedOrganizationUser> newUsers, IEnumerable<string> removeUserExternalIds,
            bool overwriteExisting);
        Task RotateApiKeyAsync(Organization organization);
        Task DeleteSsoUserAsync(Guid userId, Guid? organizationId);
    }
}
