using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Models.Api;
using Bit.Core.Services;
using Bit.Core;

namespace Bit.Api.Controllers
{
    [Route("api/settings")]
    [Authorize("Application")]
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private Guid userId => _currentContext.UserId;


        public SettingsController(
            ISessionContext authorized,
            IUserService userService            
            )
        {
            _userService = userService;
            _currentContext = authorized;
        }

        [HttpGet("domains")]
        public async Task<DomainsResponseModel> GetDomains(bool excluded = true)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var response = new DomainsResponseModel(user, excluded);
            return response;
        }

        [HttpPut("domains")]
        [HttpPost("domains")]
        public async Task<DomainsResponseModel> PutDomains([FromBody]UpdateDomainsRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            await _userService.SaveUserAsync(model.ToUser(user), true);

            var response = new DomainsResponseModel(user);
            return response;
        }
    }
}
