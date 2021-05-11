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
using Bit.Core.Models;
using System.Collections.Generic;

namespace Bit.Api.Controllers
{
    [ApiController]
    [Route("api/organizations/{orgId}/collections")]
    [Authorize("Application")]
    public class CollectionsController : ControllerBase
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICollectionService _collectionService;
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private Guid userId => _currentContext.UserId;

        public CollectionsController(
            ICollectionRepository collectionRepository,
            ICollectionService collectionService,
            IUserService userService,
            ISessionContext currentContext)
        {
            _collectionRepository = collectionRepository;
            _collectionService = collectionService;
            _userService = userService;
            _currentContext = currentContext;
        }

        [HttpGet("{id}")]
        [HttpGet("{id}/details")]
        public async Task<CollectionResponseModel> Get(Guid orgId, Guid id)
        {
            var collection = await GetCollectionAsync(id,orgId);
            return new CollectionResponseModel(collection);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<CollectionResponseModel>> Get(Guid orgId)
        {
            if (!_currentContext.ManageAllCollections(orgId) && !_currentContext.ManageUsers(orgId))
            {
                throw new NotFoundException();
            }

            var membership = _currentContext.GetMembership(orgId);
            var collections = await _collectionRepository.GetManyAsync(membership);
            var responses = collections.Select(c => new CollectionResponseModel(c));
            return new ListResponseModel<CollectionResponseModel>(responses);
        }

        [HttpGet("~/collections")]
        public async Task<ListResponseModel<CollectionDetailsResponseModel>> GetUser(Guid orgId)
        {
            var membership = _currentContext.GetMembership(orgId);
            var collections = await _collectionRepository.GetManyAsync(membership);                
            var responses = collections.Select(c => new CollectionDetailsResponseModel(c));
            return new ListResponseModel<CollectionDetailsResponseModel>(responses);
        }

        [HttpGet("{id}/users")]
        public async Task<IEnumerable<CollectionUserResponseModel>> GetUsers(Guid orgId, Guid id)
        {
            var collectionUsers = await _collectionRepository.GetAssignments(id);
            var responses = collectionUsers.Select(cu => new CollectionUserResponseModel(cu));
            return responses;
        }

        [HttpPost("")]
        public async Task<CollectionResponseModel> Post(string orgId, [FromBody]CollectionRequestModel model)
        {
            var orgIdGuid = new Guid(orgId);
            if (!ManageAnyCollections(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var collection = model.ToCollection(orgIdGuid);
            await _collectionService.SaveAsync(collection);                
            return new CollectionResponseModel(collection);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<CollectionResponseModel> Put(Guid orgId, Guid id, [FromBody]CollectionRequestModel model)
        {
            var collection = await GetCollectionAsync(id,orgId);
            await _collectionService.SaveAsync(model.ToCollection(collection));
            return new CollectionResponseModel(collection);
        }

        [HttpPut("{id}/users")]
        public async Task PutUsers(Guid orgId, Guid id, [FromBody]IEnumerable<CollectionUserRequestModel> model)
        {
            var collection = await GetCollectionAsync(id,orgId);
            await _collectionRepository.UpdateMembersAsync(model?.Select(g => g.ToCollectionMember(id)));
            await _collectionRepository.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string orgId, string id)
        {
            var collection = await GetCollectionAsync(new Guid(id), new Guid(orgId));
            await _collectionService.DeleteAsync(collection);
        }

        [HttpDelete("{id}/user/{orgUserId}")]
        [HttpPost("{id}/delete-user/{orgUserId}")]
        public async Task Delete(string orgId, string id, string orgUserId)
        {
            var collection = await GetCollectionAsync(new Guid(id), new Guid(orgId));
            await _collectionService.DeleteUserAsync(collection, new Guid(orgUserId));
        }

        private async Task<Collection> GetCollectionAsync(Guid id, Guid orgId)
        {
            if (!ManageAnyCollections(orgId))
            {
                throw new NotFoundException();
            }

            var collection = _currentContext.HasOrganizationAdminAccess(orgId) ?
                await _collectionRepository.GetByIdAsync(id) :
                await _collectionRepository.GetByIdAsync(id, _currentContext.UserId);
            if (collection == null || collection.OrganizationId != orgId)
            {
                throw new NotFoundException();
            }

            return collection;
        }

        private bool ManageAnyCollections(Guid orgId)
        {
            return _currentContext.ManageAssignedCollections(orgId) || _currentContext.ManageAllCollections(orgId);
        }
    }
}
