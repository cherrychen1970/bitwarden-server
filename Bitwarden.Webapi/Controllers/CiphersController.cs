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
        private readonly ICipherService _cipherService;
        private readonly IUserService _userService;
        private readonly ISessionContext _sessionContext;
        private readonly GlobalSettings _globalSettings;

        private Guid _sessionUserId => _sessionContext.UserId;

        public CiphersController(
            ICipherRepository cipherRepository,
            ICipherService cipherService,
            IUserService userService,
            ISessionContext currentContext,
            GlobalSettings globalSettings)
        {
            _cipherRepository = cipherRepository;
            _cipherService = cipherService;
            _userService = userService;
            _sessionContext = currentContext;
            _globalSettings = globalSettings;
        }

        [HttpGet("{id}")]
        public async Task<CipherResponseModel> Get(string id)
        {
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), _sessionUserId);
            if (cipher == null)
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
            var cipher = model.ToCipher(_sessionUserId);
            await _cipherService.SaveAsync(cipher);
            var response = new CipherResponseModel(cipher);
            return response;
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<CipherResponseModel> Put(Guid id, [FromBody] CipherRequestModel model)
        {
            var cipher = await _cipherRepository.GetByIdAsync(id, _sessionUserId);
            if (cipher == null)
            {
                throw new NotFoundException();
            }

            await _cipherService.SaveAsync(model.ToCipher(cipher));
            var response = new CipherResponseModel(cipher);
            return response;
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

        [HttpPut("{id}/partial")]
        [HttpPost("{id}/partial")]
        public async Task PutPartial(Guid id, [FromBody] CipherPartialRequestModel model)
        {
            var cipher = await _cipherRepository.GetByIdAsync(id, _sessionContext.UserId);
            if (cipher.UserId != _sessionContext.UserId)
                throw new NotFoundException();

            await _cipherRepository.UpdatePartialAsync(id, model.FolderId, model.Favorite);
        }


        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(Guid id)
        {
            var cipher = await _cipherRepository.GetByIdAsync(id, _sessionUserId);
            if (cipher == null)
                throw new NotFoundException();

            await _cipherService.DeleteAsync(cipher);
        }

        [HttpDelete("")]
        [HttpPost("delete")]
        public async Task DeleteMany([FromBody] CipherBulkDeleteRequestModel model)
        {
            await _cipherService.DeleteManyAsync(model.Ids);
        }

        [HttpPut("{id}/delete")]
        public async Task PutDelete(string id)
        {

            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), _sessionUserId);
            if (cipher == null)
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

        [HttpPut("{id}/restore")]
        public async Task<CipherResponseModel> PutRestore(string id)
        {
            var cipher = await _cipherRepository.GetByIdAsync(new Guid(id), _sessionUserId);
            if (cipher == null)
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

            var ciphers = await _cipherRepository.GetManyAsync(_sessionUserId);
            var restoringCiphers = ciphers.Where(c => cipherIdsToRestore.Contains(c.Id));

            await _cipherService.RestoreManyAsync(restoringCiphers);
            var responses = restoringCiphers.Select(c => new CipherResponseModel(c));
            return new ListResponseModel<CipherResponseModel>(responses);
        }

        [HttpPut("move")]
        [HttpPost("move")]
        public async Task MoveMany([FromBody] CipherBulkMoveRequestModel model)
        {
            await _cipherService.MoveManyAsync(model.Ids, model.FolderId);
        }


        [HttpPost("purge")]
        public async Task PostPurge([FromBody] CipherPurgeRequestModel model, string organizationId = null)
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

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                await _cipherRepository.PurgeAsync(user.Id);
            }
            else
            {
                var orgId = new Guid(organizationId);
                if (!_sessionContext.ManageAllCollections(orgId))
                {
                    throw new NotFoundException();
                }
                await _cipherService.PurgeAsync(orgId);
            }
        }
    }
}
