using System;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core;
using Bit.Core.Exceptions;
using Bit.Core.Models.Api;
using Bit.Core.Models.Api.Request;
using Bit.Core.Models.Api.Response;
using Bit.Core.Repositories;
using Bit.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Api.Controllers
{
    [Route("api/emergency-access")]
    [Authorize("Application")]
    public class EmergencyAccessController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmergencyAccessRepository _emergencyAccessRepository;
        private readonly IEmergencyAccessService _emergencyAccessService;
        private readonly ISessionContext _currentContext;

        public EmergencyAccessController(
            IUserService userService,
            IEmergencyAccessRepository emergencyAccessRepository,
            IEmergencyAccessService emergencyAccessService,
            ISessionContext currentContext
            )
        {
            _userService = userService;
            _emergencyAccessRepository = emergencyAccessRepository;
            _emergencyAccessService = emergencyAccessService;
            _currentContext = currentContext;
        }

        [HttpGet("trusted")]
        public async Task<ListResponseModel<EmergencyAccessGranteeDetailsResponseModel>> GetContacts()
        {            
            var granteeDetails = await _emergencyAccessRepository.GetManyDetailsByGrantorIdAsync(_currentContext.UserId);

            var responses = granteeDetails.Select(d =>
                new EmergencyAccessGranteeDetailsResponseModel(d));

            return new ListResponseModel<EmergencyAccessGranteeDetailsResponseModel>(responses);
        }

        [HttpGet("granted")]
        public async Task<ListResponseModel<EmergencyAccessGrantorDetailsResponseModel>> GetGrantees()
        {        
            var granteeDetails = await _emergencyAccessRepository.GetManyDetailsByGranteeIdAsync(_currentContext.UserId);

            var responses = granteeDetails.Select(d => new EmergencyAccessGrantorDetailsResponseModel(d));

            return new ListResponseModel<EmergencyAccessGrantorDetailsResponseModel>(responses);
        }

        [HttpGet("{id}")]
        public async Task<EmergencyAccessGranteeDetailsResponseModel> Get(string id)
        {            
            var result = await _emergencyAccessService.GetAsync(new Guid(id), _currentContext.UserId);
            return new EmergencyAccessGranteeDetailsResponseModel(result);
        }
        
        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task Put(string id, [FromBody]EmergencyAccessUpdateRequestModel model)
        {
            var emergencyAccess = await _emergencyAccessRepository.GetByIdAsync(new Guid(id));
            if (emergencyAccess == null)
            {
                throw new NotFoundException();
            }
            
            await _emergencyAccessService.SaveAsync(model.ToEmergencyAccess(emergencyAccess), _currentContext.UserId);
        }
        
        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string id)
        {            
            await _emergencyAccessService.DeleteAsync(new Guid(id), _currentContext.UserId);
        }
        
        [HttpPost("invite")]
        public async Task Invite([FromBody] EmergencyAccessInviteRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.InviteAsync(user, model.Email, model.Type.Value, model.WaitTimeDays);
        }

        [HttpPost("{id}/reinvite")]
        public async Task Reinvite(string id)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.ResendInviteAsync(user, new Guid(id));
        }

        [HttpPost("{id}/accept")]
        public async Task Accept(string id, [FromBody] OrganizationUserAcceptRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.AcceptUserAsync(new Guid(id), user, model.Token, _userService);
        }

        [HttpPost("{id}/confirm")]
        public async Task Confirm(string id, [FromBody] OrganizationUserConfirmRequestModel model)
        {            
            await _emergencyAccessService.ConfirmUserAsync(new Guid(id), model.Key, _currentContext.UserId);
        }

        [HttpPost("{id}/initiate")]
        public async Task Initiate(string id)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.InitiateAsync(new Guid(id), user);
        }

        [HttpPost("{id}/approve")]
        public async Task Accept(string id)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.ApproveAsync(new Guid(id), user);
        }
        
        [HttpPost("{id}/reject")]
        public async Task Reject(string id)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.RejectAsync(new Guid(id), user);
        }

        [HttpPost("{id}/takeover")]
        public async Task<EmergencyAccessTakeoverResponseModel> Takeover(string id)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            var (result, grantor) = await _emergencyAccessService.TakeoverAsync(new Guid(id), user);
            return new EmergencyAccessTakeoverResponseModel(result, grantor);
        }
        
        [HttpPost("{id}/password")]
        public async Task Password(string id, [FromBody] EmergencyAccessPasswordRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            await _emergencyAccessService.PasswordAsync(new Guid(id), user, model.NewMasterPasswordHash, model.Key);
        }
        
    }
}
