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
        private IConfigurationProvider _mapperProvider=>_mapper.ConfigurationProvider;
        private Guid _sessionUserId =>_sessionContext.UserId;

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
    
        public async Task SaveAsync(Models.Cipher cipher)
        {            
            if (!(await CanEditAsync(cipher)))
            {
                throw new BadRequestException("You do not have permissions to edit this.");
            }           

            cipher.UserId = _sessionUserId;            
            if (cipher.Id == default(Guid))
            {
                if (cipher.OrganizationId.HasValue)
                {
                    await _orgCipherRepository.CreateAsync(cipher);
                }
                else
                {
                    // Make sure the user can save new ciphers to their personal vault
                    var userPolicies = await _policyRepository.GetManyByUserIdAsync(_sessionUserId);
                    if (userPolicies != null)
                    {
                        foreach (var policy in userPolicies.Where(p => p.Enabled && p.Type == PolicyType.PersonalOwnership))
                        {
                            var org = await _organizationUserRepository.GetDetailsByUserAsync(_sessionUserId, policy.OrganizationId,
                                OrganizationUserStatusType.Confirmed);
                            if(org != null && org.Enabled && org.UsePolicies 
                               && org.Type != OrganizationUserType.Admin && org.Type != OrganizationUserType.Owner)
                            {
                                throw new BadRequestException("Due to an Enterprise Policy, you are restricted from saving items to your personal vault.");
                            }
                        }
                    }
                    await _cipherRepository.CreateAsync(cipher);
                }
                await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Created);
                await _pushService.PushSyncCipherCreateAsync(cipher, null);
            }
            else
            {                           
                await _cipherRepository.ReplaceAsync(cipher);
                await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_Updated);

                // push
                await _pushService.PushSyncCipherUpdateAsync(cipher, null);
            }
        }
 
        public async Task DeleteAsync(Models.Cipher cipher)
        {
            if (!(await CanEditAsync(cipher)))
            {
                throw new BadRequestException("You do not have permissions to delete this.");
            }

            await _cipherRepository.DeleteAsync(cipher);
            await _attachmentStorageService.DeleteAttachmentsForCipherAsync(cipher.Id);
            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Deleted);

            // push
            await _pushService.PushSyncCipherDeleteAsync(cipher);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> cipherIds, Guid? organizationId = null)
        {
            var cipherIdsSet = new HashSet<Guid>(cipherIds);
            var deletingCiphers = new List<Models.Cipher>();
            

            if (organizationId.HasValue)
            {
                var membership = _sessionContext.OrganizationMemberships.SingleOrDefault(x=>x.OrganizationId==organizationId);
                if (membership==null) throw new ForbidException();
                var ciphers = await _orgCipherRepository.GetManyAsync(membership);
                deletingCiphers = ciphers.Where(c => cipherIdsSet.Contains(c.Id)).ToList();
                await _orgCipherRepository.DeleteManyAsync(deletingCiphers.Select(c => c.Id), membership);
            }
            else
            {
                var ciphers = await _cipherRepository.GetManyAsync(_sessionUserId);
                deletingCiphers = ciphers.Where(c => cipherIdsSet.Contains(c.Id) && c.Edit).Select(x => (Cipher)x).ToList();
                await _cipherRepository.DeleteManyAsync(deletingCiphers.Select(c => c.Id), _sessionUserId);
            }

            var events = deletingCiphers.Select(c =>
                new Tuple<Models.Cipher, EventType, DateTime?>(c, EventType.Cipher_Deleted, null));
            foreach (var eventsBatch in events.Batch(100))
            {
                await _eventService.LogCipherEventsAsync(eventsBatch);
            }

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
            await _orgCipherRepository.PurgeAsync( _sessionContext.GetMembership(organizationId));
            await _eventService.LogOrganizationEventAsync(org, Enums.EventType.Organization_PurgedVault);
        }

        public async Task MoveManyAsync(IEnumerable<Guid> cipherIds, Guid? destinationFolderId)
        {
            var movingUserId = _sessionContext.UserId;
            if (destinationFolderId.HasValue)
            {
                var folder = await _folderRepository.GetByIdAsync(destinationFolderId.Value);
                if (folder == null || folder.UserId != movingUserId)
                {
                    throw new BadRequestException("Invalid folder.");
                }
            }

            await _cipherRepository.MoveAsync(cipherIds, destinationFolderId, movingUserId);
            // push
            await _pushService.PushSyncCiphersAsync(movingUserId);
        }
  
          public async Task SaveFolderAsync(Folder folder)
        {
            if (folder.Id == default(Guid))
            {
                await _folderRepository.CreateAsync(folder);

                // push
                await _pushService.PushSyncFolderCreateAsync(folder);
            }
            else
            {
                folder.RevisionDate = DateTime.UtcNow;
                await _folderRepository.UpsertAsync(folder);

                // push
                await _pushService.PushSyncFolderUpdateAsync(folder);
            }
        }

        public async Task DeleteFolderAsync(Folder folder)
        {
            await _folderRepository.DeleteAsync(folder);

            // push
            await _pushService.PushSyncFolderDeleteAsync(folder);
        }
        public async Task SaveCollectionsAsync(Models.Cipher cipher, Guid collectionId)            
        {
            if ( ! _sessionContext.ManageAllCollections(cipher.OrganizationId.Value))        
                throw new BadRequestException("You do not have permissions to soft delete this.");                    

            //await _orgCipherRepository.UpdateCollectionsAsync(cipher, collectionIds);
            var cipherEntity = await _orgCipherRepository.GetEntityAsync(cipher.Id,_sessionContext.GetMembership(cipher.OrganizationId.Value));
            cipherEntity.CollectionId = collectionId;
            await _orgCipherRepository.SaveChangesAsync();
            /*
            await _eventService.LogCipherEventAsync(cipher, Enums.EventType.Cipher_UpdatedCollections);
            await _pushService.PushSyncCipherUpdateAsync(cipher, collectionIds);
            */
        }

        public async Task ImportCiphersAsync(
            List<Folder> folders,
            List<Cipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> folderRelationships)
        {
            foreach (var cipher in ciphers)
            {
                cipher.SetNewId();
            }

            // Init. ids for folders
            foreach (var folder in folders)
            {
                folder.SetNewId();
            }

            // Create the folder associations based on the newly created folder ids
            foreach (var relationship in folderRelationships)
            {
                var cipher = ciphers.ElementAtOrDefault(relationship.Key);
                var folder = folders.ElementAtOrDefault(relationship.Value);

                if (cipher == null || folder == null)
                {
                    continue;
                }
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
        }

        // TODO :test incomplete
        public async Task ImportCiphersAsync(
            List<Collection> collections,
            List<Cipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> collectionRelationships,
            Guid importingUserId)
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
            await _pushService.PushSyncVaultAsync(importingUserId);
        }

        public async Task SoftDeleteAsync(Models.Cipher cipher)
        {
            if (!(await CanEditAsync(cipher)))
            {
                throw new BadRequestException("You do not have permissions to soft delete this.");
            }

            if (cipher.DeletedDate.HasValue)
            {
                // Already soft-deleted, we can safely ignore this
                return;
            }
            

            if (cipher is Cipher details)
            {
                await _cipherRepository.UpsertAsync(details);
            }
            else
            {
                await _cipherRepository.UpsertAsync(cipher);
            }
            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_SoftDeleted);

            // push
            await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }

        public async Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds, Guid? organizationId)
        {
            var cipherIdsSet = new HashSet<Guid>(cipherIds);
            var deletingCiphers = new List<Models.Cipher>();

            if (organizationId.HasValue)
            {
                var membership = _sessionContext.GetMembership(organizationId.Value);
                var ciphers = await _orgCipherRepository.GetManyAsync(membership);
                deletingCiphers = ciphers.Where(c => cipherIdsSet.Contains(c.Id)).ToList();
                await _orgCipherRepository.SoftDeleteManyAsync(deletingCiphers.Select(c => c.Id), membership);
            }
            else
            {
                var ciphers = await _cipherRepository.GetManyAsync(_sessionUserId);
                deletingCiphers = ciphers.Where(c => cipherIdsSet.Contains(c.Id) && c.Edit).Select(x => (Cipher)x).ToList();
                await _cipherRepository.SoftDeleteManyAsync(deletingCiphers.Select(c =>  c.Id), _sessionUserId);
            }

            var events = deletingCiphers.Select(c =>
                new Tuple<Models.Cipher, EventType, DateTime?>(c, EventType.Cipher_SoftDeleted, null));
            foreach (var eventsBatch in events.Batch(100))
            {
                await _eventService.LogCipherEventsAsync(eventsBatch);
            }

            // push
            await _pushService.PushSyncCiphersAsync(_sessionUserId);
        }

        public async Task RestoreAsync(Models.Cipher cipher)
        {
            if ((await CanEditAsync(cipher)))
            {
                throw new BadRequestException("You do not have permissions to delete this.");
            }

            if (!cipher.DeletedDate.HasValue)
            {
                // Already restored, we can safely ignore this
                return;
            }

            cipher.DeletedDate = null;            

            if (cipher is Cipher details)
            {
                await _cipherRepository.UpsertAsync(details);
            }
            else
            {
                await _cipherRepository.UpsertAsync(cipher);
            }
            await _eventService.LogCipherEventAsync(cipher, EventType.Cipher_Restored);

            // push
            await _pushService.PushSyncCipherUpdateAsync(cipher, null);
        }

        public async Task RestoreManyAsync(IEnumerable<Cipher> ciphers, Guid restoringUserId)
        {
            var revisionDate = await _cipherRepository.RestoreManyAsync(ciphers.Select(c => c.Id), restoringUserId);

            var events = ciphers.Select(c =>
            {                
                c.DeletedDate = null;
                return new Tuple<Cipher, EventType, DateTime?>(c, EventType.Cipher_Restored, null);
            });
            foreach (var eventsBatch in events.Batch(100))
            {
                await _eventService.LogCipherEventsAsync(eventsBatch);
            }

            // push
            await _pushService.PushSyncCiphersAsync(restoringUserId);
        }

        private async Task<bool> CanEditAsync(Models.Cipher cipher)
        {
            if (!cipher.OrganizationId.HasValue && cipher.UserId.HasValue && cipher.UserId.Value == _sessionUserId)            
                return true;

            if ( cipher.OrganizationId.HasValue  && _sessionContext.ManageAllCollections(cipher.OrganizationId.Value))            
                return true;
            // user cipher
            var membership = _sessionContext.GetMembership(cipher.OrganizationId.Value);
            var cipherToCompare =await _orgCipherRepository.GetByIdAsync(cipher.Id, membership);
            return cipherToCompare.Edit;
        }

        private void ValidateCipherLastKnownRevisionDateAsync(Models.Cipher cipher, DateTime? lastKnownRevisionDate)
        {
            // cherry...
            return;
            if (cipher.Id == default || !lastKnownRevisionDate.HasValue)
            {
                return;
            }

            if ((cipher.RevisionDate - lastKnownRevisionDate.Value).Duration() > TimeSpan.FromSeconds(1))
            {
                throw new BadRequestException(
                    "The cipher you are updating is out of date. Please save your work, sync your vault, and try again."
                );
            }
        }
    }
}
