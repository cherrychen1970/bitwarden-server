using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Bit.Core.Models.Api;
using Bit.Core.Exceptions;
using Bit.Core.Services;
using Bit.Core;
using Bit.Api.Utilities;
using Bit.Core.Utilities;
using System.Collections.Generic;
using Bit.Core.Models;

namespace Bit.Api.Controllers
{
    [Route("api/ciphers")]
    [Authorize("Application")]
    public class CiphersController : Controller
    {
        private readonly ICipherRepository _cipherRepository;
        private readonly IOrganizationCipherRepository _orgCipherRepository;
        private readonly ICollectionCipherRepository _collectionCipherRepository;
        private readonly ICipherService _cipherService;
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private readonly GlobalSettings _globalSettings;

        private Guid userId => _currentContext.UserId;

        public CiphersController(
            ICipherRepository cipherRepository,
            IOrganizationCipherRepository orgCipherRepository,
            ICollectionCipherRepository collectionCipherRepository,
            ICipherService cipherService,
            IUserService userService,
            ISessionContext currentContext,
            GlobalSettings globalSettings)
        {
            _orgCipherRepository = orgCipherRepository;
            _cipherRepository = cipherRepository;
            _collectionCipherRepository = collectionCipherRepository;
            _cipherService = cipherService;
            _userService = userService;
            _currentContext = currentContext;
            _globalSettings = globalSettings;
        }

        [HttpGet("{id}")]
        public async Task<CipherResponseModel> Get(string id)
        {         
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            return new CipherResponseModel(cipher);
        }

        [HttpGet("{id}/admin")]
        public async Task<CipherResponseModel> GetAdmin(string id)
        {
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            return new CipherResponseModel(cipher);
        }

        [HttpGet("{id}/full-details")]
        [HttpGet("{id}/details")]
        public async Task<CipherDetailsResponseModel> GetDetails(string id)
        {            
            var cipherId = new Guid(id);
            var cipher = await _cipherRepository.GetByIdAsync(cipherId, userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            var collectionCiphers = await _collectionCipherRepository.GetManyByUserIdCipherIdAsync(userId, cipherId);
            return new CipherDetailsResponseModel(cipher);
        }

        // TODO : what's this
        [HttpGet("")]
        public async Task<ListResponseModel<CipherDetailsResponseModel>> Get()
        {            
            var hasOrgs = _currentContext.HasOrganizations()            ;
            // TODO: Use hasOrgs proper for cipher listing here?
            var ciphers = await _cipherRepository.GetManyAsync(userId);
            Dictionary<Guid, IGrouping<Guid, CollectionCipher>> collectionCiphersGroupDict = null;
            if (hasOrgs)
            {
                var collectionCiphers = await _collectionCipherRepository.GetManyByUserIdAsync(userId);
                collectionCiphersGroupDict = collectionCiphers.GroupBy(c => c.CipherId).ToDictionary(s => s.Key);
            }

            var responses = ciphers.Select(c => new CipherDetailsResponseModel(c)).ToList();                
            return new ListResponseModel<CipherDetailsResponseModel>(responses);
        }

        [HttpPost("")]
        public async Task<CipherResponseModel> Post([FromBody]CipherRequestModel model)
        {            
            var cipher = model.ToCipherDetails();
            if (cipher.OrganizationId.HasValue && !_currentContext.IsOrganizationMember(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPost("admin")]
        public async Task<CipherResponseModel> PostAdmin([FromBody]CipherCreateRequestModel model)
        {            
            var cipher = model.Cipher.ToCipherDetails();
            if (!_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }
            cipher.CollectionId = model.CollectionIds.First();
            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<CipherResponseModel> Put(string id, [FromBody]CipherRequestModel model)
        {            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            if (cipher.OrganizationId != model.OrganizationId)
            {
                throw new BadRequestException("Organization mismatch. Re-sync if you recently shared this item, " +
                    "then try again.");
            }

            await _cipherService.SaveAsync(model.ToCipher(cipher));

            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPut("{id}/admin")]
        [HttpPost("{id}/admin")]
        public async Task<CipherResponseModel> PutAdmin(string id, [FromBody]CipherRequestModel model)
        {            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            // object cannot be a descendant of CipherDetails, so let's clone it.
            var cipherClone = model.ToCipher(cipher).Clone();
            await _cipherService.SaveAsync(cipherClone);

            var response = new CipherResponseModel(cipherClone);
            return response;
        }

        [HttpGet("organization-details")]
        public async Task<ListResponseModel<CipherDetailsResponseModel>> GetOrganizationCollections(
            Guid organizationId)
        {            
            if (!_currentContext.ManageAllCollections(organizationId) && !_currentContext.AccessReports(organizationId))
            {
                throw new NotFoundException();
            }

            var membership = _currentContext.OrganizationMemberships.SingleOrDefault(x=>x.OrganizationId==organizationId);
            var ciphers = await _orgCipherRepository.GetManyAsync(membership);
            var responses = ciphers.Select(c => new CipherDetailsResponseModel(c));
            return new ListResponseModel<CipherDetailsResponseModel>(responses);
        }

        [HttpPost("import")]
        public async Task PostImport([FromBody]ImportCiphersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Ciphers.Count() > 6000 || model.FolderRelationships.Count() > 6000 ||
                    model.Folders.Count() > 1000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            
            var folders = model.Folders.Select(f => f.ToFolder(userId)).ToList();
            var ciphers = model.Ciphers.Select(c => c.ToCipherDetails(userId)).ToList();            

            await _cipherService.ImportCiphersAsync(folders, ciphers, model.FolderRelationships);
        }

        [HttpPost("import-organization")]
        public async Task PostImport([FromQuery]string organizationId,
            [FromBody]ImportOrganizationCiphersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Ciphers.Count() > 6000 || model.CollectionRelationships.Count() > 12000 ||
                    model.Collections.Count() > 1000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            var orgId = new Guid(organizationId);
            if (!_currentContext.AccessImportExport(orgId))
            {
                throw new NotFoundException();
            }

            
            var collections = model.Collections.Select(c => c.ToCollection(orgId)).ToList();
            var ciphers = model.Ciphers.Select(x=>x.ToCipherDetails()).ToList();
            ciphers.ForEach(x=>x.OrganizationId=orgId);
            await _cipherService.ImportCiphersAsync(collections, ciphers, model.CollectionRelationships, userId);
        }

        [HttpPut("{id}/partial")]
        [HttpPost("{id}/partial")]
        public async Task PutPartial(Guid id, [FromBody]CipherPartialRequestModel model)
        {
            
            var folderId = string.IsNullOrWhiteSpace(model.FolderId) ? null : (Guid?)new Guid(model.FolderId);
            var cipher = await _cipherRepository.GetByIdAsync(id,_currentContext.UserId);
            if (!cipher.UserId.HasValue || cipher.UserId!=_currentContext.UserId)
            {
                throw new NotFoundException();
            }
            await _cipherRepository.UpdatePartialAsync(cipher, folderId, model.Favorite);
        }

        [HttpPut("{id}/collections")]
        [HttpPost("{id}/collections")]
        public async Task PutCollections(Guid id, [FromBody]CipherCollectionsRequestModel model)
        {            
            var cipher = await _cipherRepository.GetByIdAsync(id, userId);
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.IsOrganizationMember(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveCollectionsAsync(cipher, model.CollectionIds.First());
        }

        [HttpPut("{id}/collections-admin")]
        [HttpPost("{id}/collections-admin")]
        public async Task PutCollectionsAdmin(string id, [FromBody]CipherCollectionsRequestModel model)
        {
            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveCollectionsAsync(cipher,  model.CollectionIds.First());
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string id)
        {
            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            await _cipherService.DeleteAsync(cipher);
        }

        [HttpDelete("{id}/admin")]
        [HttpPost("{id}/delete-admin")]
        public async Task DeleteAdmin(string id)
        {
            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.DeleteAsync(cipher);
        }

        [HttpDelete("")]
        [HttpPost("delete")]
        public async Task DeleteMany([FromBody]CipherBulkDeleteRequestModel model)
        {
            await _cipherService.DeleteManyAsync(model.Ids);
        }

        [HttpDelete("admin")]
        [HttpPost("delete-admin")]
        public async Task DeleteManyAdmin([FromBody]CipherBulkDeleteRequestModel model)
        {
            if (model == null || !model.OrganizationId.HasValue || !_currentContext.ManageAllCollections(model.OrganizationId.Value))
                throw new NotFoundException();
           
            await _cipherService.DeleteManyAsync(model.Ids, model.OrganizationId);
        }

        [HttpPut("{id}/delete")]
        public async Task PutDelete(string id)
        {
            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }
            await _cipherService.SoftDeleteAsync(cipher);
        }

        [HttpPut("{id}/delete-admin")]
        public async Task PutDeleteAdmin(string id)
        {
            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SoftDeleteAsync(cipher);
        }

        [HttpPut("delete")]
        public async Task PutDeleteMany([FromBody]CipherBulkDeleteRequestModel model)
        {
            await _cipherService.SoftDeleteManyAsync(model.Ids);
        }

        [HttpPut("delete-admin")]
        public async Task PutDeleteManyAdmin([FromBody]CipherBulkDeleteRequestModel model)
        {
            if (model == null || !model.OrganizationId.HasValue || !_currentContext.ManageAllCollections(model.OrganizationId.Value))            
            {
                throw new NotFoundException();
            }
            
            await _cipherService.SoftDeleteManyAsync(model.Ids, model.OrganizationId);
        }

        [HttpPut("{id}/restore")]
        public async Task<CipherResponseModel> PutRestore(string id)
        {
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), userId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            await _cipherService.RestoreAsync(cipher);
            return new CipherResponseModel(cipher);
        }

        [HttpPut("{id}/restore-admin")]
        public async Task<CipherResponseModel> PutRestoreAdmin(string id)
        {            
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !cipher.OrganizationId.HasValue ||
                !_currentContext.ManageAllCollections(cipher.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.RestoreAsync(cipher);
            return new CipherResponseModel(cipher);
        }

        [HttpPut("restore")]
        public async Task<ListResponseModel<CipherResponseModel>> PutRestoreMany([FromBody] CipherBulkRestoreRequestModel model)
        {
            var cipherIdsToRestore = new HashSet<Guid>(model.Ids);

            var ciphers = await _cipherRepository.GetManyAsync(userId);
            var restoringCiphers = ciphers.Where(c => cipherIdsToRestore.Contains(c.Id) && c.Edit);

            await _cipherService.RestoreManyAsync(restoringCiphers, userId);
            var responses = restoringCiphers.Select(c => new CipherResponseModel(c));
            return new ListResponseModel<CipherResponseModel>(responses);
        }

        [HttpPut("move")]
        [HttpPost("move")]
        public async Task MoveMany([FromBody]CipherBulkMoveRequestModel model)
        {
            await _cipherService.MoveManyAsync(model.Ids,model.FolderId);                
        }

  
        [HttpPost("purge")]
        public async Task PostPurge([FromBody]CipherPurgeRequestModel model, string organizationId = null)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                ModelState.AddModelError("MasterPasswordHash", "Invalid password.");
                await Task.Delay(2000);
                throw new BadRequestException(ModelState);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                await _cipherRepository.PurgeAsync(user.Id);
            }
            else
            {
                var orgId = new Guid(organizationId);
                if (!_currentContext.ManageAllCollections(orgId))
                {
                    throw new NotFoundException();
                }
                await _cipherService.PurgeAsync(orgId);
            }
        }
    }
}
