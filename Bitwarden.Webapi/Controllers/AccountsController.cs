using Bit.Api.Utilities;
using Bit.Core;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using Bit.Core.Models.Api;
using Bit.Core.Models.Api.Request.Accounts;
using Bit.Core.Models.Business;
using Bit.Core.Models.Data;
using Bit.Core.Models;
using Bit.Core.Repositories;
using Bit.Core.Services;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bit.Api.Controllers
{
    [Route("api/accounts")]
    [Authorize("Application")]
    //[Authorize(AuthenticationSchemes="Bearer")]
    public class AccountsController : Controller
    {
        private readonly GlobalSettings _globalSettings;
        private readonly ICipherRepository _cipherRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IOrganizationService _organizationService;
        private readonly IOrganizationUserRepository _organizationUserRepository;        
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly ISessionContext _currentContext;
        private Guid userId => _currentContext.UserId;

        public AccountsController(
            GlobalSettings globalSettings,
            ICipherRepository cipherRepository,
            IFolderRepository folderRepository,
            IOrganizationService organizationService,
            IOrganizationUserRepository organizationUserRepository,            
            ISsoUserRepository ssoUserRepository,
            IUserRepository userRepository,
            ISessionContext currentContext,
            IUserService userService)
        {
            _cipherRepository = cipherRepository;
            _folderRepository = folderRepository;
            _globalSettings = globalSettings;
            _organizationService = organizationService;
            _organizationUserRepository = organizationUserRepository;            
            _userRepository = userRepository;
            _userService = userService;
            _currentContext = currentContext;
        }


        [HttpGet("test")]
        public IActionResult test()
        {
            var claims = HttpContext.User.Claims.GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(v => v.Value));
            return Ok(claims);
        }    

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(HttpContext.User.Identities.Select(x=>x.AuthenticationType));
        }        

        [HttpPost("prelogin")]
        [AllowAnonymous]
        public async Task<PreloginResponseModel> PostPrelogin([FromBody] PreloginRequestModel model)
        {
            var kdfInformation = await _userRepository.GetKdfInformationByEmailAsync(model.Email);
            if (kdfInformation == null)
            {
                kdfInformation = new UserKdfInformation
                {
                    Kdf = KdfType.PBKDF2_SHA256,
                    KdfIterations = 100000
                };
            }
            return new PreloginResponseModel(kdfInformation);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task PostRegister([FromBody] RegisterRequestModel model)
        {
            var result = await _userService.RegisterUserAsync(model.ToUser(), model.MasterPasswordHash,
                model.Token, model.OrganizationUserId);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors.Where(e => e.Code != "DuplicateUserName"))
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("password-hint")]
        [AllowAnonymous]
        public async Task PostPasswordHint([FromBody] PasswordHintRequestModel model)
        {
            await _userService.SendMasterPasswordHintAsync(model.Email);
        }

        [HttpPost("email-token")]
        public async Task PostEmailToken([FromBody] EmailTokenRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (!await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                await Task.Delay(2000);
                throw new BadRequestException("MasterPasswordHash", "Invalid password.");
            }

            await _userService.InitiateEmailChangeAsync(user, model.NewEmail);
        }

        [HttpPost("email")]
        public async Task PostEmail([FromBody] EmailRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.ChangeEmailAsync(user, model.MasterPasswordHash, model.NewEmail,
                model.NewMasterPasswordHash, model.Token, model.Key);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("verify-email")]
        public async Task PostVerifyEmail()
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            await _userService.SendEmailVerificationAsync(user);
        }

        [HttpPost("verify-email-token")]
        [AllowAnonymous]
        public async Task PostVerifyEmailToken([FromBody] VerifyEmailRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(new Guid(model.UserId));
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            var result = await _userService.ConfirmEmailAsync(user, model.Token);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("password")]
        public async Task PostPassword([FromBody] PasswordRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.ChangePasswordAsync(user, model.MasterPasswordHash,
                model.NewMasterPasswordHash, model.Key);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("set-password")]
        public async Task PostSetPasswordAsync([FromBody] SetPasswordRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.SetPasswordAsync(model.ToUser(user), model.MasterPasswordHash, model.Key);
            //model.OrgIdentifier);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            throw new BadRequestException(ModelState);
        }

        [HttpPost("verify-password")]
        public async Task PostVerifyPassword([FromBody] VerifyPasswordRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException(_currentContext.UserId.ToString());
            }

            if (await _userService.CheckPasswordAsync(user, model.MasterPasswordHash))
            {
                return;
            }

            ModelState.AddModelError(nameof(model.MasterPasswordHash), "Invalid password.");
            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("kdf")]
        public async Task PostKdf([FromBody] KdfRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.ChangeKdfAsync(user, model.MasterPasswordHash,
                model.NewMasterPasswordHash, model.Key, model.Kdf.Value, model.KdfIterations.Value);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("key")]
        public async Task PostKey([FromBody] UpdateKeyRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var existingCiphers = await _cipherRepository.GetManyAsync(user.Id);
            var ciphersDict = model.Ciphers?.ToDictionary(c => c.Id.Value);
            var ciphers = new List<UserCipher>();
            if (existingCiphers.Any() && ciphersDict != null)
            {
                foreach (var cipher in existingCiphers.Where(c => ciphersDict.ContainsKey(c.Id)))
                {
                    ciphers.Add(ciphersDict[cipher.Id].ToCipher(cipher));
                }
            }

            var existingFolders = await _folderRepository.GetManyByUserIdAsync(user.Id);
            var foldersDict = model.Folders?.ToDictionary(f => f.Id);
            var folders = new List<Folder>();
            if (existingFolders.Any() && foldersDict != null)
            {
                foreach (var folder in existingFolders.Where(f => foldersDict.ContainsKey(f.Id)))
                {
                    folders.Add(foldersDict[folder.Id].ToFolder(folder));
                }
            }

            var result = await _userService.UpdateKeyAsync(
                user,
                model.MasterPasswordHash,
                model.Key,
                model.PrivateKey,
                ciphers,
                folders);

            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpPost("security-stamp")]
        public async Task PostSecurityStamp([FromBody] SecurityStampRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.RefreshSecurityStampAsync(user, model.MasterPasswordHash);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }

        [HttpGet("profile")]
        public async Task<ProfileResponseModel> GetProfile()
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var organizationMembershipProfiles = await _organizationUserRepository.GetManyAsync(_currentContext.OrganizationMemberships);
            var response = new ProfileResponseModel(user, organizationMembershipProfiles,
                await _userService.TwoFactorIsEnabledAsync(user));
            return response;
        }

        [HttpGet("organizations")]
        public async Task<ListResponseModel<ProfileOrganizationResponseModel>> GetOrganizations()
        {
            throw new NotImplementedException();
            /*
            var organizationUserDetails = await _organizationUserRepository.GetManyDetailsByUserAsync(_currentContext.UserId,
                OrganizationUserStatusType.Confirmed);
            var responseData = organizationUserDetails.Select(o => new ProfileOrganizationResponseModel(o));
            return new ListResponseModel<ProfileOrganizationResponseModel>(responseData);
            */
        }

        [HttpPut("profile")]
        [HttpPost("profile")]
        public async Task<ProfileResponseModel> PutProfile([FromBody] UpdateProfileRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            await _userService.SaveUserAsync(model.ToUser(user));
            var response = new ProfileResponseModel(user, null, await _userService.TwoFactorIsEnabledAsync(user));
            return response;
        }

        [HttpGet("revision-date")]
        public async Task<long?> GetAccountRevisionDate()
        {
            var date = await _userService.GetAccountRevisionDateByIdAsync(_currentContext.UserId);
            return CoreHelpers.ToEpocMilliseconds(date);
        }

        [HttpPost("keys")]
        public async Task<KeysResponseModel> PostKeys([FromBody] KeysRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            await _userService.SaveUserAsync(model.ToUser(user));
            return new KeysResponseModel(user);
        }

        [HttpGet("keys")]
        public async Task<KeysResponseModel> GetKeys()
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            return new KeysResponseModel(user);
        }

        [HttpDelete]
        [HttpPost("delete")]
        public async Task Delete([FromBody] DeleteAccountRequestModel model)
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
            }
            else
            {
                var result = await _userService.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return;
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            throw new BadRequestException(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("delete-recover")]
        public async Task PostDeleteRecover([FromBody] DeleteRecoverRequestModel model)
        {
            await _userService.SendDeleteConfirmationAsync(model.Email);
        }

        [HttpPost("delete-recover-token")]
        [AllowAnonymous]
        public async Task PostDeleteRecoverToken([FromBody] VerifyDeleteRecoverRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(new Guid(model.UserId));
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _userService.DeleteAsync(user, model.Token);
            if (result.Succeeded)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await Task.Delay(2000);
            throw new BadRequestException(ModelState);
        }


        [HttpGet("enterprise-portal-signin-token")]
        [Authorize("Web")]
        public async Task<string> GetEnterprisePortalSignInToken()
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var token = await _userService.GenerateEnterprisePortalSignInTokenAsync(user);
            if (token == null)
            {
                throw new BadRequestException("Cannot generate sign in token.");
            }

            return token;
        }


        [HttpPost("api-key")]
        public async Task<ApiKeyResponseModel> ApiKey([FromBody] ApiKeyRequestModel model)
        {
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
                var response = new ApiKeyResponseModel(user);
                return response;
            }
        }

        [HttpPost("rotate-api-key")]
        public async Task<ApiKeyResponseModel> RotateApiKey([FromBody] ApiKeyRequestModel model)
        {
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
                await _userService.RotateApiKeyAsync(user);
                var response = new ApiKeyResponseModel(user);
                return response;
            }
        }
    }
}
