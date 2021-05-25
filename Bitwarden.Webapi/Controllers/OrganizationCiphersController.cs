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
    [ApiController]
    [Route("api/organizations/{orgId}/ciphers")]    
    [Authorize("Application")]
    public class OrganizationCiphersController : ControllerBase
    {        
        private readonly IOrganizationCipherRepository _orgCipherRepository;
        private readonly ICollectionCipherRepository _collectionCipherRepository;
        private readonly ICipherService _cipherService;
        private readonly IUserService _userService;
        private readonly ISessionContext _sessionContext;
        private readonly GlobalSettings _globalSettings;

        private Guid _sessionUserId => _sessionContext.UserId;

        public OrganizationCiphersController(            
            IOrganizationCipherRepository orgCipherRepository,
            ICollectionCipherRepository collectionCipherRepository,
            ICipherService cipherService,
            IUserService userService,
            ISessionContext currentContext,
            GlobalSettings globalSettings)
        {
            _orgCipherRepository = orgCipherRepository;            
            _collectionCipherRepository = collectionCipherRepository;
            _cipherService = cipherService;
            _userService = userService;
            _sessionContext = currentContext;
            _globalSettings = globalSettings;
        }

        [HttpGet("{id}/admin")]
        public async Task<CipherResponseModel> GetAdmin(string id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            return new CipherResponseModel(cipher);
        }

        // TODO : what's this
        [HttpGet("")]
        public async Task<ListResponseModel<CipherDetailsResponseModel>> Get()
        {
            throw new NotImplementedException();
            /*
            var hasOrgs = _sessionContext.HasOrganizations();
            // TODO: Use hasOrgs proper for cipher listing here?
            var ciphers = await _cipherRepository.GetManyAsync(_sessionUserId);
            Dictionary<Guid, IGrouping<Guid, CollectionCipher>> collectionCiphersGroupDict = null;
            if (hasOrgs)
            {
                var collectionCiphers = await _collectionCipherRepository.GetManyByUserIdAsync(_sessionUserId);
                collectionCiphersGroupDict = collectionCiphers.GroupBy(c => c.CipherId).ToDictionary(s => s.Key);
            }

            var responses = ciphers.Select(c => new CipherDetailsResponseModel(c)).ToList();
            return new ListResponseModel<CipherDetailsResponseModel>(responses);
            */
        }

        [HttpPost("")]
        public async Task<CipherResponseModel> Post([FromBody] CipherRequestModel model)
        {
            var cipher = model.ToOrganizationCipher();
            if (_sessionContext.IsOrganizationMember(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPost("admin")]
        public async Task<CipherResponseModel> PostAdmin([FromBody] CipherCreateRequestModel model)
        {
            var cipher = model.Cipher.ToOrganizationCipher();
            if (!_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }
            if (model.CollectionIds.Any())
                cipher.CollectionId = model.CollectionIds.First();
            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPut("{id}/admin")]
        [HttpPost("{id}/admin")]
        public async Task<CipherResponseModel> PutAdmin(string id, [FromBody] CipherRequestModel model)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new NotFoundException();

            await _cipherService.SaveAsync(model.ToOrganizationCipher(cipher));
            var response = new CipherResponseModel(cipher);
            return response;
        }

        // weird name.... it's requesting organization ciphers
        [HttpGet("organization-details")]
        public async Task<ListResponseModel<CipherDetailsResponseModel>> GetOrganizationCollections(
            Guid organizationId)
        {
            /*
            if (!_sessionContext.ManageAllCollections(organizationId) && !_sessionContext.AccessReports(organizationId))
            {
                throw new NotFoundException();
            }
            */

            var membership = _sessionContext.OrganizationMemberships.SingleOrDefault(x => x.OrganizationId == organizationId);
            var ciphers = await _orgCipherRepository.GetManyAsync(membership);
            var responses = ciphers.Select(c => new CipherDetailsResponseModel(c));
            return new ListResponseModel<CipherDetailsResponseModel>(responses);
        }

        [HttpPost("import")]
        public async Task PostImport([FromBody] ImportCiphersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Ciphers.Count() > 6000 || model.FolderRelationships.Count() > 6000 ||
                    model.Folders.Count() > 1000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            var folders = model.Folders.Select(f => f.ToFolder(_sessionUserId)).ToList();
            var ciphers = model.Ciphers.Select(c => c.ToCipher(_sessionUserId)).ToList();

            await _cipherService.ImportCiphersAsync(folders, ciphers, model.FolderRelationships);
        }

        [HttpPost("import-organization")]
        public async Task PostImport([FromQuery] string organizationId,
            [FromBody] ImportOrganizationCiphersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Ciphers.Count() > 6000 || model.CollectionRelationships.Count() > 12000 ||
                    model.Collections.Count() > 1000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            var orgId = new Guid(organizationId);
            if (!_sessionContext.AccessImportExport(orgId))
            {
                throw new NotFoundException();
            }


            var collections = model.Collections.Select(c => c.ToCollection(orgId)).ToList();
            var ciphers = model.Ciphers.Select(x => x.ToOrganizationCipher()).ToList();
            ciphers.ForEach(x => x.OrganizationId = orgId);
            await _cipherService.ImportCiphersAsync(collections, ciphers, model.CollectionRelationships);
        }


        [HttpPut("{id}/collections")]
        [HttpPost("{id}/collections")]
        public async Task PutCollections(Guid id, [FromBody] CipherCollectionsRequestModel model)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.IsOrganizationMember(cipher.OrganizationId))
                throw new NotFoundException();

            await _cipherService.SaveCollectionsAsync(cipher, model.CollectionIds.First());
        }

        [HttpPut("{id}/collections-admin")]
        [HttpPost("{id}/collections-admin")]
        public async Task PutCollectionsAdmin(string id, [FromBody] CipherCollectionsRequestModel model)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new NotFoundException();
            await _cipherService.SaveCollectionsAsync(cipher, model.CollectionIds.First());
        }

        [HttpDelete("{id}/admin")]
        [HttpPost("{id}/delete-admin")]
        public async Task DeleteAdmin(Guid id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new NotFoundException();

            await _cipherService.DeleteAsync(cipher);
        }

        [HttpDelete("")]
        [HttpPost("delete")]
        public async Task DeleteMany([FromBody] CipherBulkDeleteRequestModel model)
        {
            await _cipherService.DeleteManyAsync(model.Ids);
        }

        [HttpDelete("admin")]
        [HttpPost("delete-admin")]
        public async Task DeleteManyAdmin([FromBody] CipherBulkDeleteRequestModel model)
        {
            if (model == null || !_sessionContext.ManageAllCollections(model.OrganizationId.Value))
                throw new NotFoundException();

            await _cipherService.DeleteManyAsync(model.Ids, model.OrganizationId.Value);
        }

        [HttpPut("{id}/delete-admin")]
        public async Task PutDeleteAdmin(Guid id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            await _cipherService.SoftDeleteAsync(cipher);
        }

        [HttpPut("delete")]
        public async Task PutDeleteMany([FromBody] CipherBulkDeleteRequestModel model)
        {
            await _cipherService.SoftDeleteManyAsync(model.Ids);
        }

        [HttpPut("delete-admin")]
        public async Task PutDeleteManyAdmin([FromBody] CipherBulkDeleteRequestModel model)
        {
            if (model == null || !model.OrganizationId.HasValue || !_sessionContext.ManageAllCollections(model.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SoftDeleteManyAsync(model.Ids, model.OrganizationId.Value);
        }

        [HttpPut("{id}/restore-admin")]
        public async Task<CipherResponseModel> PutRestoreAdmin(Guid id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            await _cipherService.RestoreAsync(cipher);
            return new CipherResponseModel(cipher);
        }

        [HttpPost("purge")]
        public async Task PostPurge([FromBody] CipherPurgeRequestModel model, Guid orgId)
        {
            var user = await _userService.GetUserByIdAsync(_sessionContext.UserId);
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


                if (!_sessionContext.ManageAllCollections(orgId))
                {
                    throw new NotFoundException();
                }
                await _cipherService.PurgeAsync(orgId);
            
        }
    }
}
