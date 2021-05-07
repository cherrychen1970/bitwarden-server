using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Bit.Core;
using Bit.Core.Repositories;
using Bit.Core.Models.Api;
using Bit.Core.Exceptions;
using Bit.Core.Services;
using Bit.Core.Models.Business;

namespace Bit.Api.Controllers
{
    [Route("api/organizations/{orgId}/users")]
    [Authorize("Application")]
    public class OrganizationUsersController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IOrganizationService _organizationService;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private readonly IMapper _mapper;
        private IConfigurationProvider _mapperProvider=>_mapper.ConfigurationProvider;

        public OrganizationUsersController(
            IMapper mapper,
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IOrganizationService organizationService,
            ICollectionRepository collectionRepository,
            IGroupRepository groupRepository,
            IUserService userService,
            ISessionContext currentContext)
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _organizationService = organizationService;
            _collectionRepository = collectionRepository;
            _groupRepository = groupRepository;
            _userService = userService;
            _currentContext = currentContext;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<OrganizationUserDetailsResponseModel> Get(string orgId, string id)
        {
            var organizationUser = await _organizationUserRepository.GetByIdWithCollectionsAsync(new Guid(id));
            if (organizationUser == null || !_currentContext.ManageUsers(organizationUser.Item1.OrganizationId))
            {
                throw new NotFoundException();
            }

            return new OrganizationUserDetailsResponseModel(organizationUser.Item1, organizationUser.Item2);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<OrganizationUserUserDetailsResponseModel>> Get(string orgId)
        {
            var orgGuidId = new Guid(orgId);
            var ou = await _organizationUserRepository.GetByOrganizationAsync(orgGuidId,_currentContext.UserId);
            //if (!_currentContext.ManageAssignedCollections(orgGuidId) && !_currentContext.ManageGroups(orgGuidId))
            if (ou.Type!=Core.Enums.OrganizationUserType.Admin && ou.Type!=Core.Enums.OrganizationUserType.Owner)
            {
                throw new Exception(ou.Type.ToString());
            }

            var organizationUsers = await _organizationUserRepository.GetManyDetailsByOrganizationAsync(orgGuidId);
            var responseTasks = organizationUsers.Select(async o => new OrganizationUserUserDetailsResponseModel(o,
                await _userService.TwoFactorIsEnabledAsync(o)));
            var responses = await Task.WhenAll(responseTasks);
            return new ListResponseModel<OrganizationUserUserDetailsResponseModel>(responses);
        }

        [HttpGet("{id}/groups")]
        public async Task<IEnumerable<string>> GetGroups(string orgId, string id)
        {
            var organizationUser = await _organizationUserRepository.GetByIdAsync(new Guid(id));
            if (organizationUser == null || !_currentContext.ManageGroups(organizationUser.OrganizationId))
            {
                throw new NotFoundException();
            }

            var groupIds = await _groupRepository.GetManyIdsByUserIdAsync(organizationUser.Id);
            var responses = groupIds.Select(g => g.ToString());
            return responses;
        }

        [HttpPost("invite")]
        public async Task Invite(string orgId, [FromBody]OrganizationUserInviteRequestModel model)
        {
            var orgGuidId = new Guid(orgId);
            if (!_currentContext.ManageUsers(orgGuidId))
            {
                throw new NotFoundException();
            }

            // TODO : cherry test this mapping
            var invite = _mapper.Map<OrganizationUserInvite>(model);
           
            var result = await _organizationService.InviteUserAsync(orgGuidId, _currentContext.UserId, null, invite);
        }

        [HttpPost("{id}/reinvite")]
        public async Task Reinvite(Guid orgId, Guid id)
        {            
            if (!_currentContext.ManageUsers(orgId))
            {
                throw new NotFoundException();
            }
            
            await _organizationService.ResendInviteAsync(orgId, _currentContext.UserId, id);
        }

        [HttpPost("{id}/accept")]
        public async Task Accept(string orgId, string id, [FromBody]OrganizationUserAcceptRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            var result = await _organizationService.AcceptUserAsync(new Guid(id), user, model.Token, _userService);
        }

        [HttpPost("{id}/confirm")]
        public async Task Confirm(string orgId, string id, [FromBody]OrganizationUserConfirmRequestModel model)
        {
            var orgGuidId = new Guid(orgId);
            if (!_currentContext.ManageUsers(orgGuidId))
            {
                throw new NotFoundException();
            }
            
            var result = await _organizationService.ConfirmUserAsync(orgGuidId, new Guid(id), model.Key, _currentContext.UserId,
                _userService);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task Put(string orgId, string id, [FromBody]OrganizationUserUpdateRequestModel model)
        {
            var orgGuidId = new Guid(orgId);
            if (!_currentContext.ManageUsers(orgGuidId))
            {
                throw new NotFoundException();
            }

            var organizationUser = await _organizationUserRepository.GetByIdAsync(new Guid(id));
            if (organizationUser == null || organizationUser.OrganizationId != orgGuidId)
            {
                throw new NotFoundException();
            }
            
            await _organizationService.SaveUserAsync(model.ToOrganizationUser(organizationUser), _currentContext.UserId,
                model.Collections?.Select(c => c.ToSelectionReadOnly()));
        }

        [HttpPut("{id}/groups")]
        [HttpPost("{id}/groups")]
        public async Task PutGroups(string orgId, string id, [FromBody]OrganizationUserUpdateGroupsRequestModel model)
        {
            var orgGuidId = new Guid(orgId);
            if (!_currentContext.ManageUsers(orgGuidId))
            {
                throw new NotFoundException();
            }

            var organizationUser = await _organizationUserRepository.GetByIdAsync(new Guid(id));
            if (organizationUser == null || organizationUser.OrganizationId != orgGuidId)
            {
                throw new NotFoundException();
            }
            
            await _organizationService.UpdateUserGroupsAsync(organizationUser, model.GroupIds.Select(g => new Guid(g)), _currentContext.UserId);
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string orgId, string id)
        {
            var orgGuidId = new Guid(orgId);
            if (!_currentContext.ManageUsers(orgGuidId))
            {
                throw new NotFoundException();
            }

            await _organizationService.DeleteUserAsync(orgGuidId, new Guid(id), _currentContext.UserId);
        }
    }
}
