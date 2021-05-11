using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

using Bit.Core.Models;
using Bit.Core.Repositories;
using Bit.Core.Exceptions;
using Bit.Core.Enums;
using Bit.Core.Utilities;

namespace Bit.Core.Services
{
    public class CipherService : ICipherService
    {
        private readonly ICipherRepository _cipherRepository;
        private readonly IOrganizationCipherRepository _orgCipherRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly ICollectionCipherRepository _collectionCipherRepository;
        private readonly IPushNotificationService _pushService;
        private readonly IAttachmentStorageService _attachmentStorageService;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IPolicyRepository _policyRepository;
        private readonly GlobalSettings _globalSettings;
        private readonly ISessionContext _sessionContext;
        private readonly IMapper _mapper;
        private IConfigurationProvider _mapperProvider => _mapper.ConfigurationProvider;
        private Guid _sessionUserId => _sessionContext.UserId;

        public CipherService(
            IMapper mapper,
            ICipherRepository cipherRepository,
            IOrganizationCipherRepository orgCipherRepository,
            IFolderRepository folderRepository,
            ICollectionRepository collectionRepository,
            IUserRepository userRepository,
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            ICollectionCipherRepository collectionCipherRepository,
            IPushNotificationService pushService,
            IAttachmentStorageService attachmentStorageService,
            IEventService eventService,
            IUserService userService,
            IPolicyRepository policyRepository,
            ISessionContext sessionContext,
            GlobalSettings globalSettings)
        {
            _mapper = mapper;
            _cipherRepository = cipherRepository;
            _orgCipherRepository = orgCipherRepository;
            _folderRepository = folderRepository;
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _collectionCipherRepository = collectionCipherRepository;
            _pushService = pushService;
            _attachmentStorageService = attachmentStorageService;
            _eventService = eventService;
            _userService = userService;
            _policyRepository = policyRepository;
            _sessionContext = sessionContext;
            _globalSettings = globalSettings;
        }

        public async Task SaveAsync(Models.OrganizationCipher cipher)
        {
            if (!(await CanEditAsync(cipher)))
            {
                throw new BadRequestException("You do not have permissions to edit this.");
            }
            if (cipher.Id == default(Guid))
            {                
                await _orgCipherRepository.CreateAsync(cipher);
                await _cipherRepository.SaveChangesAsync();
                //await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Created);
                //await _pushService.PushSyncCipherCreateAsync(cipher, null);
            }
            else
            {
                await _orgCipherRepository.ReplaceAsync(cipher);
                await _cipherRepository.SaveChangesAsync();
                //await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Updated);
                //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
            }
        }
        public async Task SaveAsync(Models.UserCipher cipher)
        {
            cipher.UserId = _sessionUserId;
            if (cipher.Id == default(Guid))
            {                
                await _cipherRepository.CreateAsync(cipher);
                await _cipherRepository.SaveChangesAsync();
                //await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Created);
                //await _pushService.PushSyncCipherCreateAsync(cipher, null);
            }
            else
            {
                await _cipherRepository.ReplaceAsync(cipher);
                await _cipherRepository.SaveChangesAsync();
                //await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Updated);
                //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
            }
            
        }

        public async Task DeleteAsync(Models.UserCipher cipher)
        {
            if (cipher.UserId != _sessionUserId)
                throw new BadRequestException("You do not have permissions to delete this.");

            await _cipherRepository.DeleteAsync(cipher);
            await _cipherRepository.SaveChangesAsync();
            //await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Deleted);
            // push
            await _pushService.PushSyncCipherDeleteAsync(cipher);
        }

        public async Task DeleteAsync(Models.OrganizationCipher cipher)
        {
            if (!(await CanEditAsync(cipher)))
                throw new BadRequestException("You do not have permissions to edit this.");

            await _orgCipherRepository.DeleteAsync(cipher);
            await _cipherRepository.SaveChangesAsync();
            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Deleted);
            // push
            //await _pushService.PushSyncCipherDeleteAsync(cipher);
        }
        public async Task DeleteManyAsync(IEnumerable<Guid> cipherIds)
        {
            await _cipherRepository.DeleteManyAsync(cipherIds, _sessionUserId);
            await _cipherRepository.SaveChangesAsync();
            /*
                        var events = deletingCiphers.Select(c =>
                            new Tuple<Models.Cipher, EventType, DateTime?>(c, EventType.Cipher_Deleted, null));
                        foreach (var eventsBatch in events.Batch(100))
                        {
                            await _eventService.LogCipherEventsAsync(eventsBatch);
                        }
            */
            // push
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }
        public async Task DeleteManyAsync(IEnumerable<Guid> cipherIds, Guid organizationId)
        {
            var cipherIdsSet = new HashSet<Guid>(cipherIds);
            var deletingCiphers = new List<Models.Cipher>();

            var membership = _sessionContext.GetMembership(organizationId);
            if (membership == null) throw new ForbidException();
            await _orgCipherRepository.DeleteManyAsync(cipherIds, membership);
            await _cipherRepository.SaveChangesAsync();

            /*
                        var events = deletingCiphers.Select(c =>
                            new Tuple<Models.Cipher, EventType, DateTime?>(c, EventType.Cipher_Deleted, null));
                        foreach (var eventsBatch in events.Batch(100))
                        {
                            await _eventService.LogCipherEventsAsync(eventsBatch);
                        }
            */
            // push
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }

        public async Task PurgeAsync(Guid organizationId)
        {
            var org = await _organizationRepository.GetByIdAsync(organizationId);
            if (org == null)
            {
                throw new NotFoundException();
            }
            await _orgCipherRepository.PurgeAsync(_sessionContext.GetMembership(organizationId));
            await _cipherRepository.SaveChangesAsync();
            await _eventService.LogOrganizationEventAsync(org, Enums.EventType.Organization_PurgedVault);
        }

        public async Task MoveManyAsync(IEnumerable<Guid> cipherIds, Guid destinationFolderId)
        {
            var folder = await _folderRepository.GetByIdAsync(destinationFolderId);
            if (folder == null || folder.UserId != _sessionUserId)
            {
                throw new BadRequestException("Invalid folder.");
            }

            await _cipherRepository.MoveAsync(cipherIds, destinationFolderId, _sessionUserId);
            await _cipherRepository.SaveChangesAsync();
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }

        public async Task SaveFolderAsync(Folder folder)
        {
            if (folder.Id == default(Guid))
            {
                await _folderRepository.CreateAsync(folder);
                await _folderRepository.SaveChangesAsync();
                // push
                await _pushService.PushSyncFolderCreateAsync(folder);
            }
            else
            {                
                await _folderRepository.ReplaceAsync(folder);
                await _folderRepository.SaveChangesAsync();
                // push
                await _pushService.PushSyncFolderUpdateAsync(folder);
            }
        }

        public async Task DeleteFolderAsync(Folder folder)
        {
            await _folderRepository.DeleteAsync(folder);
            await _folderRepository.SaveChangesAsync();

            // push
            await _pushService.PushSyncFolderDeleteAsync(folder);
        }
        public async Task SaveCollectionsAsync(Models.OrganizationCipher cipher, Guid collectionId)
        {
            if (!_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new BadRequestException("You do not have permissions to soft delete this.");

            //await _orgCipherRepository.UpdateCollectionsAsync(cipher, collectionIds);
            var cipherEntity = await _orgCipherRepository.GetEntityAsync(cipher.Id, _sessionContext.GetMembership(cipher.OrganizationId));
            cipherEntity.CollectionId = collectionId;
            await _orgCipherRepository.SaveChangesAsync();
            /*
            await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_UpdatedCollections);
            await _pushService.PushSyncCipherUpdateAsync(cipher, collectionIds);
            */
        }

        public async Task ImportCiphersAsync(
            List<Folder> folders,
            List<UserCipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> folderRelationships)
        {
            // Create the folder associations based on the newly created folder ids
            /*
            foreach (var relationship in folderRelationships)
            {
                var cipher = ciphers.ElementAtOrDefault(relationship.Key);
                var folder = folders.ElementAtOrDefault(relationship.Value);

                if (cipher == null || folder == null)
                {
                    continue;
                }
                cipher.FolderId=folder.Id;
                // TODO : cherry fix this.
                throw new NotImplementedException();
                //                cipher.Folders = $"{{\"{cipher.UserId.ToString().ToUpperInvariant()}\":" +   $"\"{folder.Id.ToString().ToUpperInvariant()}\"}}";
            }

            // Create it all
            await _cipherRepository.CreateAsync(ciphers, folders);

            // push
            var userId = folders.FirstOrDefault()?.UserId ?? ciphers.FirstOrDefault()?.UserId;
            if (userId.HasValue)
            {
                await _pushService.PushSyncVaultAsync(userId.Value);
            }
            */
            throw new NotImplementedException();
        }

        // TODO :test incomplete
        public async Task ImportCiphersAsync(
            List<Collection> collections,
            List<OrganizationCipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> collectionRelationships
            )
        {
            // Create associations based on the newly assigned ids
            var collectionCiphers = new List<CollectionCipher>();
            foreach (var relationship in collectionRelationships)
            {
                var cipher = ciphers.ElementAtOrDefault(relationship.Key);
                var collection = collections.ElementAtOrDefault(relationship.Value);

                if (cipher == null || collection == null)
                    continue;

                cipher.CollectionId = collection.Id;
            }
            foreach (var item in collections)
            {
                await _collectionRepository.CreateAsync(item);
            }

            await _orgCipherRepository.CreateAsync(ciphers);
            await _orgCipherRepository.SaveChangesAsync();

            // push
            await _pushService.PushSyncVaultAsync(_sessionUserId);
        }

        public async Task SoftDeleteAsync(Models.UserCipher cipher)
        {
            if (cipher.UserId != _sessionUserId)
                throw new BadRequestException("You do not have permissions to delete this.");

            if (cipher.DeletedDate.HasValue)
                return;

            await _cipherRepository.ReplaceAsync(cipher);
            await _cipherRepository.SaveChangesAsync();
            //await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_SoftDeleted);
            //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }

        public async Task SoftDeleteAsync(Models.OrganizationCipher cipher)
        {
            if (!(await CanEditAsync(cipher)))
                throw new BadRequestException("You do not have permissions to soft delete this.");

            if (cipher.DeletedDate.HasValue)
                return;

            var cipherEntity = await _orgCipherRepository.GetEntityAsync(cipher.Id, _sessionContext.GetMembership(cipher.OrganizationId));
            cipherEntity.DeletedDate = DateTime.UtcNow;
            await _cipherRepository.SaveChangesAsync();

            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_SoftDeleted);
            //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }
        public async Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds)
        {
            await _cipherRepository.SoftDeleteManyAsync(cipherIds, _sessionUserId);
            await _cipherRepository.SaveChangesAsync();
        }
        public async Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds, Guid organizationId)
        {
            var cipherIdsSet = new HashSet<Guid>(cipherIds);
            var deletingCiphers = new List<Models.Cipher>();

            var membership = _sessionContext.GetMembership(organizationId);
            await _orgCipherRepository.SoftDeleteManyAsync(cipherIds, membership);
            await _cipherRepository.SaveChangesAsync();

            /*
                        var events = deletingCiphers.Select(c =>
                            new Tuple<Models.OrganizationCipher, EventType, DateTime?>(c, EventType.Cipher_SoftDeleted, null));
                        foreach (var eventsBatch in events.Batch(100))
                        {
                            await _eventService.LogCipherEventsAsync(eventsBatch);
                        }
            */
            // push
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }

        public async Task RestoreAsync(Models.UserCipher cipher)
        {
            if (cipher.UserId != _sessionUserId)
                throw new BadRequestException("You do not have permissions to delete this.");
            if (!cipher.DeletedDate.HasValue)
                return;
            await _cipherRepository.SoftDeleteAsync(cipher.Id, _sessionUserId);
            await _cipherRepository.SaveChangesAsync();
            //await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Restored);
            //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }

        public async Task RestoreAsync(Models.OrganizationCipher cipher)
        {
            if ((await CanEditAsync(cipher)))
                throw new BadRequestException("You do not have permissions to delete this.");

            if (!cipher.DeletedDate.HasValue)
                return;

            var cipherEntity = await _orgCipherRepository.GetEntityAsync(cipher.Id, _sessionContext.GetMembership(cipher.OrganizationId));
            cipherEntity.DeletedDate = null;
            await _orgCipherRepository.SaveChangesAsync();
            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Restored);
            //await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }
        public async Task RestoreManyAsync(IEnumerable<UserCipher> ciphers)
        {
            await _cipherRepository.RestoreManyAsync(ciphers.Select(c => c.Id), _sessionUserId);
            await _cipherRepository.SaveChangesAsync();
            /*
            var events = ciphers.Select(c =>
            {
                c.DeletedDate = null;
                return new Tuple<Cipher, EventType, DateTime?>(c, EventType.Cipher_Restored, null);
            });
            
            foreach (var eventsBatch in events.Batch(100))
            {
                await _eventService.LogCipherEventsAsync(eventsBatch);
            }
            */
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }

        private async Task<bool> CanEditAsync(Models.OrganizationCipher cipher)
        {
            if (_sessionContext.ManageAllCollections(cipher.OrganizationId))
                return true;
            // user cipher
            var membership = _sessionContext.GetMembership(cipher.OrganizationId);
            var cipherToCompare = await _orgCipherRepository.GetByIdAsync(cipher.Id, membership);
            return cipherToCompare.Edit;
        }
    }
}
