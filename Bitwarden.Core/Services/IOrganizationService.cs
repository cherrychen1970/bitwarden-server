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
        Task<Tuple<Organization, OrganizationMembershipProfile>> SignUpAsync(OrganizationSignup organizationSignup);
        Task DeleteAsync(Organization organization);
        Task UpdateAsync(Organization organization);
        Task<List<OrganizationMembershipProfile>> InviteUserAsync(Guid organizationId, OrganizationUserInvite orgUserInvite);
        Task ResendInviteAsync(OrganizationMembershipProfile user);
        Task<OrganizationMembershipProfile> AcceptUserAsync(OrganizationMembershipProfile user, string token=null);
        Task<OrganizationMembershipProfile> ConfirmUserAsync(OrganizationMembershipProfile user, string key);
        Task SaveUserAsync(OrganizationMembershipProfile user);
        Task DeleteUserAsync(OrganizationMembershipProfile user);
        Task RotateApiKeyAsync(Organization organization);
    }
}
