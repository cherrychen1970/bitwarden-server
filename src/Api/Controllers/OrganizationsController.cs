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

        [HttpGet("{id}/license")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task<OrganizationLicense> GetLicense(string id, [FromQuery]Guid installationId)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var license = await _organizationService.GenerateLicenseAsync(orgIdGuid, installationId);
            if (license == null)
            {
                throw new NotFoundException();
            }

            return license;
        }

        [HttpGet("")]
        public async Task<ListResponseModel<ProfileOrganizationResponseModel>> GetUser()
        {            
            var organizations = await _organizationUserRepository.GetManyDetailsByUserAsync(_currentContext.UserId,
                OrganizationUserStatusType.Confirmed);
            var responses = organizations.Select(o => new ProfileOrganizationResponseModel(o));
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

        [HttpPost("license")]
        [SelfHosted(SelfHostedOnly = true)]
        public async Task<OrganizationResponseModel> PostLicense(OrganizationCreateLicenseRequestModel model)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var license = await ApiHelpers.ReadJsonFileFromBody<OrganizationLicense>(HttpContext, model.License);
            if (license == null)
            {
                throw new BadRequestException("Invalid license");
            }

            var policies = await _policyRepository.GetManyByUserIdAsync(user.Id);
            if (policies.Any(policy => policy.Type == PolicyType.SingleOrg))
            {
                throw new Exception("You may not create an organization. You belong to an organization " +
                     "which has a policy that prohibits you from being a member of any other organization.");
            }

            var result = await _organizationService.SignUpAsync(license, user, model.Key, model.CollectionName);
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

            var updatebilling = !_globalSettings.SelfHosted && (model.BusinessName != organization.BusinessName ||
                model.BillingEmail != organization.BillingEmail);

            await _organizationService.UpdateAsync(model.ToOrganization(organization, _globalSettings), updatebilling);
            return new OrganizationResponseModel(organization);
        }

        [HttpPost("{id}/payment")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task PostPayment(string id, [FromBody]PaymentRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            await _organizationService.ReplacePaymentMethodAsync(orgIdGuid, model.PaymentToken,
                model.PaymentMethodType.Value, new TaxInfo
                {
                    BillingAddressLine1 = model.Line1,
                    BillingAddressLine2 = model.Line2,
                    BillingAddressState = model.State,
                    BillingAddressCity = model.City,
                    BillingAddressPostalCode = model.PostalCode,
                    BillingAddressCountry = model.Country,
                    TaxIdNumber = model.TaxId,
                });
        }

        [HttpPost("{id}/upgrade")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task<PaymentResponseModel> PostUpgrade(string id, [FromBody]OrganizationUpgradeRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var result = await _organizationService.UpgradePlanAsync(orgIdGuid, model.ToOrganizationUpgrade());
            return new PaymentResponseModel
            {
                Success = result.Item1,
                PaymentIntentClientSecret = result.Item2
            };
        }

        [HttpPost("{id}/seat")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task<PaymentResponseModel> PostSeat(string id, [FromBody]OrganizationSeatRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var result = await _organizationService.AdjustSeatsAsync(orgIdGuid, model.SeatAdjustment.Value);
            return new PaymentResponseModel
            {
                Success = true,
                PaymentIntentClientSecret = result
            };
        }

        [HttpPost("{id}/storage")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task<PaymentResponseModel> PostStorage(string id, [FromBody]StorageRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var result = await _organizationService.AdjustStorageAsync(orgIdGuid, model.StorageGbAdjustment.Value);
            return new PaymentResponseModel
            {
                Success = true,
                PaymentIntentClientSecret = result
            };
        }

        [HttpPost("{id}/verify-bank")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task PostVerifyBank(string id, [FromBody]OrganizationVerifyBankRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            await _organizationService.VerifyBankAsync(orgIdGuid, model.Amount1.Value, model.Amount2.Value);
        }

        [HttpPost("{id}/cancel")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task PostCancel(string id)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            await _organizationService.CancelSubscriptionAsync(orgIdGuid);
        }

        [HttpPost("{id}/reinstate")]
        [SelfHosted(NotSelfHostedOnly = true)]
        public async Task PostReinstate(string id)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            await _organizationService.ReinstateSubscriptionAsync(orgIdGuid);
        }

        [HttpPost("{id}/leave")]
        public async Task Leave(string id)
        {
            var orgGuidId = new Guid(id);
            if (!_currentContext.IsOrganizationMember(orgGuidId))
            {
                throw new NotFoundException();
            }

            await _organizationService.DeleteUserAsync(orgGuidId, _currentContext.UserId);
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

        [HttpPost("{id}/license")]
        [SelfHosted(SelfHostedOnly = true)]
        public async Task PostLicense(string id, LicenseRequestModel model)
        {
            var orgIdGuid = new Guid(id);
            if (!_currentContext.IsOrganizationOwner(orgIdGuid))
            {
                throw new NotFoundException();
            }

            var license = await ApiHelpers.ReadJsonFileFromBody<OrganizationLicense>(HttpContext, model.License);
            if (license == null)
            {
                throw new BadRequestException("Invalid license");
            }

            await _organizationService.UpdateLicenseAsync(new Guid(id), license);
        }

        [HttpPost("{id}/import")]
        public async Task Import(string id, [FromBody]ImportOrganizationUsersRequestModel model)
        {
            if (!_globalSettings.SelfHosted &&
                (model.Groups.Count() > 2000 || model.Users.Count(u => !u.Deleted) > 2000))
            {
                throw new BadRequestException("You cannot import this much data at once.");
            }

            var orgIdGuid = new Guid(id);
            if (!_currentContext.HasOrganizationAdminAccess(orgIdGuid))
            {
                throw new NotFoundException();
            }
            
            await _organizationService.ImportAsync(
                orgIdGuid,
                _currentContext.UserId,
                model.Groups.Select(g => g.ToImportedGroup(orgIdGuid)),
                model.Users.Where(u => !u.Deleted).Select(u => u.ToImportedOrganizationUser()),
                model.Users.Where(u => u.Deleted).Select(u => u.ExternalId),
                model.OverwriteExisting);
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
