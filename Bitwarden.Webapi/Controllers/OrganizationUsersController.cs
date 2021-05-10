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
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private readonly IMapper _mapper;
        private IConfigurationProvider _mapperProvider => _mapper.ConfigurationProvider;

        public OrganizationUsersController(
            IMapper mapper,
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IOrganizationService organizationService,
            ICollectionRepository collectionRepository,
            IUserService userService,
            ISessionContext currentContext)
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _organizationService = organizationService;
            _collectionRepository = collectionRepository;
            _userService = userService;
            _currentContext = currentContext;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<OrganizationUserResponseModel> Get(Guid orgId, Guid id)
        {
            var organizationUser = await _organizationUserRepository.GetByIdAsync(id);
            if (organizationUser == null || !_currentContext.ManageUsers(organizationUser.OrganizationId))
            {
                throw new NotFoundException();
            }
            var collections = await _collectionRepository.GetAssignments(_currentContext.GetMembership(orgId));

            return new OrganizationUserResponseModel(organizationUser, collections);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<OrganizationUserResponseModel>> GetUsers(Guid orgId)
        {
            if (!_currentContext.ManageUsers(orgId))
                throw new UnauthorizedAccessException();

            var organizationUsers = await _organizationUserRepository.GetManyByOrganizationAsync(orgId);
            var responses = organizationUsers.Select(o => new OrganizationUserResponseModel(o));
            return new ListResponseModel<OrganizationUserResponseModel>(responses);
        }

        [HttpPost("invite")]
        public async Task Invite(Guid orgId, [FromBody] OrganizationUserInviteRequestModel model)
        {
            if (!_currentContext.ManageUsers(orgId))
            {
                throw new NotFoundException();
            }

            // TODO : cherry test this mapping
            var invite = _mapper.Map<OrganizationUserInvite>(model);
            var result = await _organizationService.InviteUserAsync(orgId, invite);
        }

        [HttpPost("{id}/reinvite")]
        public async Task Reinvite(Guid orgId, Guid id)
        {
            if (!_currentContext.ManageUsers(orgId))
                throw new NotFoundException();

            var ou = await _organizationUserRepository.GetByIdAsync(id);
            if (ou == null)
                throw new NotFoundException();

            await _organizationService.ResendInviteAsync(ou);
        }

        [HttpPost("{id}/accept")]
        public async Task Accept(Guid orgId, Guid id, [FromBody] OrganizationUserAcceptRequestModel model)
        {
            var ou = await _organizationUserRepository.GetByIdAsync(id);
            if (ou == null)
                throw new NotFoundException();
            var result = await _organizationService.AcceptUserAsync(ou, model.Token);
        }

        [HttpPost("{id}/confirm")]
        public async Task Confirm(Guid orgId, Guid id, [FromBody] OrganizationUserConfirmRequestModel model)
        {
            if (!_currentContext.ManageUsers(orgId))
            {
                throw new NotFoundException();
            }

            var ou = await _organizationUserRepository.GetByIdAsync(id);
            if (ou == null)
                throw new NotFoundException();

            var result = await _organizationService.ConfirmUserAsync(ou, model.Key);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task Put(Guid orgId, Guid id, [FromBody] OrganizationUserUpdateRequestModel model)
        {
            if (!_currentContext.ManageUsers(orgId))
            {
                throw new NotFoundException();
            }

            var orgUser = await _organizationUserRepository.GetByIdAsync(id);
            if (orgUser == null || orgUser.OrganizationId != orgId)
            {
                throw new NotFoundException();
            }

            await _organizationService.SaveUserAsync(model.ToOrganizationUser(orgUser));
            await _collectionRepository.UpdateUsersAsync(model.Collections?.Select(c => c.ToCollectionAssigned(orgUser)));
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(Guid orgId, Guid id)
        {
            VerifyAccess(orgId);
            var ou = await ValidateFound(id);

            await _organizationService.DeleteUserAsync(ou);
        }

        public void VerifyAccess(Guid orgId)
        {
            if (!_currentContext.ManageUsers(orgId))
                throw new ForbidException();
        }
        public async Task<Core.Models.OrganizationMembershipProfile> ValidateFound(Guid id)
        {
            var ou = await _organizationUserRepository.GetByIdAsync(id);
            if (ou == null)
                throw new NotFoundException();
            return ou;
        }
    }
}
