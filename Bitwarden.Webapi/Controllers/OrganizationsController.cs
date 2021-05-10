using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Bit.Core.Enums;
using Bit.Core.Models.Api;
using Bit.Core.Exceptions;
using Bit.Core.Services;
using Bit.Core;
using Bit.Api.Utilities;
using Bit.Core.Models.Business;
using Bit.Core.Utilities;

namespace Bit.Api.Controllers
{
    [Route("api/organizations")]
    [Authorize("Application")]
    public class OrganizationsController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;
        //private readonly IPaymentService _paymentService;
        private readonly ISessionContext _currentContext;
        private readonly GlobalSettings _globalSettings;
        private readonly IPolicyRepository _policyRepository;

        public OrganizationsController(
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IOrganizationService organizationService,
            IUserService userService,            
            ISessionContext currentContext,
            GlobalSettings globalSettings,
            IPolicyRepository policyRepository)
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _organizationService = organizationService;
            _userService = userService;            
            _currentContext = currentContext;
            _globalSettings = globalSettings;
            _policyRepository = policyRepository;
        }

        [HttpGet("{id}")]
        public async Task<OrganizationResponseModel> Get(Guid id)
        {        
             
            if (!_currentContext.IsOrganizationOwner(id))
            {
                throw new NotFoundException();
            }  
                      
            var organization = await _organizationRepository.GetByIdAsync(id);
            if (organization == null)
            {                
                throw new NotFoundException();
            }

            return new OrganizationResponseModel(organization);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<ProfileOrganizationResponseModel>> Get()
        {            
            //var organizations = await _organizationUserRepository.GetManyDetailsByUserAsync(_currentContext.UserId,
              //  OrganizationUserStatusType.Confirmed);
            var organizationUsers = await _organizationUserRepository.GetManyAsync(_currentContext.OrganizationMemberships);
            var responses = organizationUsers
                .Select(o => new ProfileOrganizationResponseModel(o));
            return new ListResponseModel<ProfileOrganizationResponseModel>(responses);
        }

        [HttpPost("")]
        //[SelfHosted(NotSelfHostedOnly = true)]
        public async Task<OrganizationResponseModel> Post([FromBody]OrganizationCreateRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var plan = StaticStore.Plans.FirstOrDefault(plan => plan.Type == model.PlanType);
            if (plan == null || plan.LegacyYear != null)
            {
                throw new Exception("Invalid plan selected.");
            }

            var policies = await _policyRepository.GetManyByUserIdAsync(user.Id);
            if (policies.Any(policy => policy.Type == PolicyType.SingleOrg))
            {
                throw new Exception("You may not create an organization. You belong to an organization " +
                     "which has a policy that prohibits you from being a member of any other organization.");
            }

            var organizationSignup = model.ToOrganizationSignup(user);
            var result = await _organizationService.SignUpAsync(organizationSignup);
            return new OrganizationResponseModel(result.Item1);
        }

          [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<OrganizationResponseModel> Put(string id, [FromBody]OrganizationUpdateRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var organization = await _organizationRepository.GetByIdAsync(orgIdGuid);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            await _organizationService.UpdateAsync(model.ToOrganization(organization, _globalSettings));
            return new OrganizationResponseModel(organization);
        }

        [HttpPost("{id}/leave")]
        public async Task Leave(Guid id)
        {
            if (!_currentContext.IsOrganizationMember(id))
            {
                throw new NotFoundException();
            }
            var ou = await _organizationUserRepository.GetOneAsync(_currentContext.GetMembership(id));
            await _organizationService.DeleteUserAsync(ou);
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string id, [FromBody]OrganizationDeleteRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var organization = await _organizationRepository.GetByIdAsync(orgIdGuid);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                await Task.Delay(2000);
                throw new BadRequestException("MasterPasswordHash", "Invalid password.");
            }
            else
            {
                await _organizationService.DeleteAsync(organization);
            }
        }

        [HttpPost("{id}/api-key")]
        public async Task<ApiKeyResponseModel> ApiKey(string id, [FromBody]ApiKeyRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var organization = await _organizationRepository.GetByIdAsync(orgIdGuid);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                await Task.Delay(2000);
                throw new BadRequestException("MasterPasswordHash", "Invalid password.");
            }
            else
            {
                var response = new ApiKeyResponseModel(organization);
                return response;
            }
        }

        [HttpPost("{id}/rotate-api-key")]
        public async Task<ApiKeyResponseModel> RotateApiKey(string id, [FromBody]ApiKeyRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var organization = await _organizationRepository.GetByIdAsync(orgIdGuid);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                await Task.Delay(2000);
                throw new BadRequestException("MasterPasswordHash", "Invalid password.");
            }
            else
            {
                await _organizationService.RotateApiKeyAsync(organization);
                var response = new ApiKeyResponseModel(organization);
                return response;
            }
        }
    }
}
