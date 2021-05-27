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

        [HttpGet("{id}")]
        public async Task<CipherResponseModel> GetCipher(Guid id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            return new CipherResponseModel(cipher);
        }

        [HttpPost()]
        public async Task<CipherResponseModel> Post(Guid orgId, [FromBody] CipherRequestModel model)
        {
            var cipher = model.ToOrganizationCipher(orgId);
            if (!_sessionContext.IsOrganizationMember(orgId))
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<CipherResponseModel> Put(Guid id, [FromBody] CipherRequestModel model)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new NotFoundException();

            await _cipherService.SaveAsync(model.ToOrganizationCipher(cipher));
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpGet()]
        public async Task<ListResponseModel<CipherDetailsResponseModel>> GetCiphers(Guid orgId)
        {
            if (!_sessionContext.IsOrganizationMember(orgId))
            {
                throw new ForbidException();
            }
            var membership = _sessionContext.OrganizationMemberships.SingleOrDefault(x => x.OrganizationId == orgId);
            var ciphers = await _orgCipherRepository.GetManyAsync(membership);
            var responses = ciphers.Select(c => new CipherDetailsResponseModel(c));
            return new ListResponseModel<CipherDetailsResponseModel>(responses);
        }

        [HttpPost("import")]
        public async Task PostImport(Guid orgId,
            [FromBody] ImportOrganizationCiphersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Ciphers.Count() > 6000 || model.CollectionRelationships.Count() > 12000 ||
                    model.Collections.Count() > 1000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            if (!_sessionContext.AccessImportExport(orgId))
            {
                throw new NotFoundException();
            }

            var collections = model.Collections.Select(c => c.ToCollection(orgId)).ToList();
            var ciphers = model.Ciphers.Select(x => x.ToOrganizationCipher(orgId)).ToList();
            ciphers.ForEach(x => x.OrganizationId = orgId);
            await _cipherService.ImportCiphersAsync(collections, ciphers, model.CollectionRelationships);
        }

        [HttpPut("{id}/collections")]
        [HttpPost("{id}/collections")]
        public async Task PutCollections(string id, [FromBody] CipherCollectionsRequestModel model)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(new Guid(id));
            if (cipher == null || !_sessionContext.IsOrganizationMember(cipher.OrganizationId))
                throw new NotFoundException();
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
                throw new NotFoundException();
            await _cipherService.SaveCollectionsAsync(cipher, model.CollectionIds.First());
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(Guid id)
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
            if (model == null || !_sessionContext.ManageAllCollections(model.OrganizationId.Value))
                throw new NotFoundException();
            await _cipherService.DeleteManyAsync(model.Ids);
        }

        [HttpPut("{id}/delete")]
        public async Task SoftDelete(Guid id)
        {
            var cipher = await _orgCipherRepository.GetByIdAsync(id);
            if (cipher == null || !_sessionContext.ManageAllCollections(cipher.OrganizationId))
            {
                throw new NotFoundException();
            }

            await _cipherService.SoftDeleteAsync(cipher);
        }

        [HttpPut("delete")]
        public async Task SoftDeleteMany([FromBody] CipherBulkDeleteRequestModel model)
        {
            if (model == null || !model.OrganizationId.HasValue || !_sessionContext.ManageAllCollections(model.OrganizationId.Value))
            {
                throw new NotFoundException();
            }

            await _cipherService.SoftDeleteManyAsync(model.Ids, model.OrganizationId.Value);
        }

        [HttpPut("{id}/restore")]
        public async Task<CipherResponseModel> Restore(Guid id)
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
        public async Task Purge([FromBody] CipherPurgeRequestModel model, Guid orgId)
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
