using System;
using System.Threading.Tasks;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.Core.Repositories;
using System.Collections.Generic;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IEventService _eventService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;

        public CollectionService(
            IEventService eventService,
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            ICollectionRepository collectionRepository,
            IUserRepository userRepository,
            IMailService mailService)
        {
            _eventService = eventService;
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
            _mailService = mailService;
        }

        public async Task SaveAsync(Collection collection, IEnumerable<Models.CollectionAssigned> groups = null)
        {
            if (collection.Id == default(Guid))
            {
                await _collectionRepository.CreateAsync(collection);
                await _eventService.LogCollectionEventAsync(collection, Enums.EventType.Collection_Created);
            }
            else
            {
                await _collectionRepository.ReplaceAsync(collection);
                await _eventService.LogCollectionEventAsync(collection, Enums.EventType.Collection_Updated);
            }
            await _collectionRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Collection collection)
        {
            await _collectionRepository.DeleteAsync(collection);
            await _eventService.LogCollectionEventAsync(collection, Enums.EventType.Collection_Deleted);
        }

        public async Task DeleteUserAsync(Collection collection, Guid organizationUserId)
        {
            var orgUser = await _organizationUserRepository.GetByIdAsync(organizationUserId);
            if (orgUser == null || orgUser.OrganizationId != collection.OrganizationId)
            {
                throw new NotFoundException();
            }
            
            await _collectionRepository.DeleteUserAsync(collection.Id, orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, Enums.EventType.OrganizationUser_Updated);
        }
    }
}
