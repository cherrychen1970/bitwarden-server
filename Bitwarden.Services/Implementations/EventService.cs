using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Repositories;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IEventWriteService _eventWriteService;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IApplicationCacheService _applicationCacheService;
        private readonly HttpContext _httpContext;
        private readonly GlobalSettings _globalSettings;

        public EventService(
            IEventWriteService eventWriteService,
            IOrganizationUserRepository organizationUserRepository,
            IApplicationCacheService applicationCacheService,            
            GlobalSettings globalSettings)
        {
            _eventWriteService = eventWriteService;
            _organizationUserRepository = organizationUserRepository;
            _applicationCacheService = applicationCacheService;            
            _globalSettings = globalSettings;
        }

        public async Task LogUserEventAsync(Guid userId, EventType type, DateTime? date = null)
        {
            var events = new List<IEvent>
            {
                new EventMessage()
                {
                    IpAddress = _httpContext?.IpAddress(),
                    DeviceType = _httpContext?.DeviceType(),
                    UserId = userId,
                    ActingUserId = userId,
                    Type = type,
                    Date = date.GetValueOrDefault(DateTime.UtcNow)
                }
            };

            var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
            var orgs = await _organizationUserRepository.GetManyByUserAsync<OrganizationMembership>(userId, true);
            var orgEvents = orgs.Where(o => CanUseEvents(orgAbilities, o.OrganizationId))
                .Select(o => new EventMessage()
                {
                    IpAddress = _httpContext?.IpAddress(),
                    DeviceType = _httpContext?.DeviceType(),
                    OrganizationId = o.OrganizationId,
                    UserId = userId,
                    ActingUserId = userId,
                    Type = type,
                    Date = DateTime.UtcNow
                });

            if (orgEvents.Any())
            {
                events.AddRange(orgEvents);
                await _eventWriteService.CreateManyAsync(events);
            }
            else
            {
                await _eventWriteService.CreateAsync(events.First());
            }
        }

        public async Task LogCipherEventAsync(Cipher cipher, EventType type, DateTime? date = null)
        {
            var e = await BuildCipherEventMessageAsync(cipher, type, date);
            if (e != null)
            {
                await _eventWriteService.CreateAsync(e);
            }
        }

        public async Task LogCipherEventsAsync(IEnumerable<Tuple<Cipher, EventType, DateTime?>> events)
        {
            var cipherEvents = new List<IEvent>();
            foreach (var ev in events)
            {
                var e = await BuildCipherEventMessageAsync(ev.Item1, ev.Item2, ev.Item3);
                if (e != null)
                {
                    cipherEvents.Add(e);
                }
            }
            await _eventWriteService.CreateManyAsync(cipherEvents);
        }

        private async Task<EventMessage> BuildCipherEventMessageAsync(Cipher cipher, EventType type, DateTime? date = null)
        {
            // Only logging organization cipher events for now.
            if (!cipher.OrganizationId.HasValue)
            {
                return null;
            }

            if (cipher.OrganizationId.HasValue)
            {
                var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
                if (!CanUseEvents(orgAbilities, cipher.OrganizationId.Value))
                {
                    return null;
                }
            }

            return new EventMessage()
            {
                IpAddress = _httpContext?.IpAddress(),
                DeviceType = _httpContext?.DeviceType(),

                OrganizationId = cipher.OrganizationId,
                UserId = cipher.OrganizationId.HasValue ? null : cipher.UserId,
                CipherId = cipher.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
        }

        public async Task LogCollectionEventAsync(Collection collection, EventType type, DateTime? date = null)
        {
            var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
            if (!CanUseEvents(orgAbilities, collection.OrganizationId))
            {
                return;
            }

            var e = new EventMessage()
            {
                IpAddress = _httpContext?.IpAddress(),
                DeviceType = _httpContext?.DeviceType(),

                OrganizationId = collection.OrganizationId,
                CollectionId = collection.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
            await _eventWriteService.CreateAsync(e);
        }

        public async Task LogGroupEventAsync(Group group, EventType type, DateTime? date = null)
        {
            var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
            if (!CanUseEvents(orgAbilities, group.OrganizationId))
            {
                return;
            }

            var e = new EventMessage()
            {
                OrganizationId = group.OrganizationId,
                GroupId = group.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
            await _eventWriteService.CreateAsync(e);
        }

        public async Task LogPolicyEventAsync(Policy policy, EventType type, DateTime? date = null)
        {
            var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
            if (!CanUseEvents(orgAbilities, policy.OrganizationId))
            {
                return;
            }

            var e = new EventMessage()
            {
                IpAddress = _httpContext?.IpAddress(),
                DeviceType = _httpContext?.DeviceType(),

                OrganizationId = policy.OrganizationId,
                PolicyId = policy.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
            await _eventWriteService.CreateAsync(e);
        }

        public async Task LogOrganizationUserEventAsync(OrganizationUser organizationUser, EventType type,
            DateTime? date = null)
        {
            var orgAbilities = await _applicationCacheService.GetOrganizationAbilitiesAsync();
            if (!CanUseEvents(orgAbilities, organizationUser.OrganizationId))
            {
                return;
            }

            var e = new EventMessage()
            {
                IpAddress = _httpContext?.IpAddress(),
                DeviceType = _httpContext?.DeviceType(),

                OrganizationId = organizationUser.OrganizationId,
                UserId = organizationUser.UserId,
                OrganizationUserId = organizationUser.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
            await _eventWriteService.CreateAsync(e);
        }

        public async Task LogOrganizationEventAsync(Organization organization, EventType type, DateTime? date = null)
        {
            if (!organization.Enabled || !organization.UseEvents)
            {
                return;
            }

            var e = new EventMessage()
            {
                IpAddress = _httpContext?.IpAddress(),
                DeviceType = _httpContext?.DeviceType(),

                OrganizationId = organization.Id,
                Type = type,
                ActingUserId = _httpContext?.UserId(),
                Date = date.GetValueOrDefault(DateTime.UtcNow)
            };
            await _eventWriteService.CreateAsync(e);
        }

        private bool CanUseEvents(IDictionary<Guid, OrganizationAbility> orgAbilities, Guid orgId)
        {
            return orgAbilities != null && orgAbilities.ContainsKey(orgId) &&
                orgAbilities[orgId].Enabled && orgAbilities[orgId].UseEvents;
        }
    }
}
