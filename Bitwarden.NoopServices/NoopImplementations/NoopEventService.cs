using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Enums;
using Bit.Core.Models;

namespace Bit.Core.Services
{
    public class NoopEventService : IEventService
    {
        public Task LogCipherEventAsync(OrganizationCipher cipher, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogCipherEventsAsync(IEnumerable<Tuple<OrganizationCipher, EventType, DateTime?>> events)
        {
            return Task.FromResult(0);
        }

        public Task LogCollectionEventAsync(Collection collection, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogPolicyEventAsync(Policy policy, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogGroupEventAsync(Group group, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogOrganizationEventAsync(Organization organization, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogOrganizationUserEventAsync(OrganizationMembershipProfile organizationUser, EventType type,
            DateTime? date = null)
        {
            return Task.FromResult(0);
        }

        public Task LogUserEventAsync(Guid userId, EventType type, DateTime? date = null)
        {
            return Task.FromResult(0);
        }
    }
}
