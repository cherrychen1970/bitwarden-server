using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Enums;
using Bit.Core.Models;

namespace Bit.Core.Services
{
    public interface IEventService
    {
        Task LogUserEventAsync(Guid userId, EventType type, DateTime? date = null);
        Task LogCipherEventAsync(OrganizationCipher cipher, EventType type, DateTime? date = null);
        Task LogCipherEventsAsync(IEnumerable<Tuple<OrganizationCipher, EventType, DateTime?>> events);
        Task LogCollectionEventAsync(Collection collection, EventType type, DateTime? date = null);
        Task LogGroupEventAsync(Group group, EventType type, DateTime? date = null);
        Task LogPolicyEventAsync(Policy policy, EventType type, DateTime? date = null);
        Task LogOrganizationUserEventAsync(OrganizationMembershipProfile organizationUser, EventType type, DateTime? date = null);
        Task LogOrganizationEventAsync(Organization organization, EventType type, DateTime? date = null);
    }
}
