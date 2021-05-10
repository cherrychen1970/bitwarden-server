using System;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core.Repositories;
using Bit.Core.Models.Business;
using Bit.Core.Models;
using Bit.Core.Utilities;
using Bit.Core.Exceptions;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Stripe;
using Bit.Core.Enums;
using Bit.Core.Models.Data;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDataProtector _dataProtector;
        private readonly IMailService _mailService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IPushRegistrationService _pushRegistrationService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IEventService _eventService;
        private readonly IInstallationRepository _installationRepository;
        private readonly IApplicationCacheService _applicationCacheService;
        private readonly IPolicyRepository _policyRepository;
        private readonly ISsoConfigRepository _ssoConfigRepository;
        private readonly ISsoUserRepository _ssoUserRepository;
        private readonly IReferenceEventService _referenceEventService;
        private readonly GlobalSettings _globalSettings;
        private readonly IMapper _mapper;
        private readonly ISessionContext _sessionContext;
        //private readonly ITaxRateRepository _taxRateRepository;

        public OrganizationService(
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            ICollectionRepository collectionRepository,
            IUserRepository userRepository,
            IDataProtectionProvider dataProtectionProvider,
            IMailService mailService,
            IPushNotificationService pushNotificationService,
            IPushRegistrationService pushRegistrationService,
            IDeviceRepository deviceRepository,
            IEventService eventService,
            IInstallationRepository installationRepository,
            IApplicationCacheService applicationCacheService,
            IPolicyRepository policyRepository,
            ISsoConfigRepository ssoConfigRepository,
            ISsoUserRepository ssoUserRepository,
            IReferenceEventService referenceEventService,
            GlobalSettings globalSettings,
            IMapper mapper,
            ISessionContext sessionContext
            //ITaxRateRepository taxRateRepository
            )
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("OrganizationServiceDataProtector");
            _mailService = mailService;
            _pushNotificationService = pushNotificationService;
            _pushRegistrationService = pushRegistrationService;
            _deviceRepository = deviceRepository;
            _eventService = eventService;
            _installationRepository = installationRepository;
            _applicationCacheService = applicationCacheService;
            _policyRepository = policyRepository;
            _ssoConfigRepository = ssoConfigRepository;
            _ssoUserRepository = ssoUserRepository;
            _referenceEventService = referenceEventService;
            _globalSettings = globalSettings;
            _mapper = mapper;
            _sessionContext = sessionContext;
            //_taxRateRepository = taxRateRepository;
        }

        public async Task<Tuple<Organization, OrganizationMembershipProfile>> SignUpAsync(OrganizationSignup signup)
        {
            var plan = StaticStore.Plans.FirstOrDefault(p => p.Type == signup.Plan && !p.Disabled);
            if (plan == null)
            {
                throw new BadRequestException("Plan not found.");
            }

            var organization = new Organization
            {
                // Pre-generate the org id so that we can save it with the Stripe subscription..
                Id = CoreHelpers.GenerateComb(),
                Name = signup.Name,
                BillingEmail = signup.BillingEmail,
                BusinessName = signup.BusinessName,
                PlanType = plan.Type,
                Seats = (short)(plan.BaseSeats),
                MaxCollections = plan.MaxCollections,
                MaxStorageGb = !plan.BaseStorageGb.HasValue ?
                    (short?)null : (short)(plan.BaseStorageGb.Value),
                UsePolicies = plan.HasPolicies,
                UseSso = plan.HasSso,
                UseGroups = plan.HasGroups,
                UseEvents = plan.HasEvents,
                UseDirectory = plan.HasDirectory,
                UseTotp = plan.HasTotp,
                Use2fa = plan.Has2fa,
                UseApi = plan.HasApi,
                SelfHost = plan.HasSelfHost,
                Plan = plan.Name,
                Gateway = null,
                ReferenceData = signup.Owner.ReferenceData,
                Enabled = true,
                LicenseKey = CoreHelpers.SecureRandomString(20),
                ApiKey = CoreHelpers.SecureRandomString(30),
            };

            var returnValue = await SignUpAsync(organization, signup.Owner.Id, signup.OwnerKey, signup.CollectionName, true);
            await _referenceEventService.RaiseEventAsync(
                new ReferenceEvent(ReferenceEventType.Signup, organization)
                {
                    PlanName = plan.Name,
                    PlanType = plan.Type,
                    Seats = returnValue.Item1.Seats,
                    Storage = returnValue.Item1.MaxStorageGb,
                });
            return returnValue;
        }

        private async Task<Tuple<Organization, OrganizationMembershipProfile>> SignUpAsync(Organization organization,
        Guid ownerId, string ownerKey, string collectionName, bool withPayment)
        {
            try
            {
                await _organizationRepository.CreateAsync(organization);
                await _applicationCacheService.UpsertOrganizationAbilityAsync(organization);

                var orgUser = new OrganizationMembershipProfile
                {
                    OrganizationId = organization.Id,
                    UserId = ownerId,
                    Key = ownerKey,
                    Type = OrganizationUserType.Owner,
                    Status = OrganizationUserStatusType.Confirmed,
                    AccessAll = true
                };

                await _organizationUserRepository.CreateAsync(orgUser);

                if (!string.IsNullOrWhiteSpace(collectionName))
                {
                    var defaultCollection = new Collection
                    {
                        Name = collectionName,
                        OrganizationId = organization.Id
                    };
                    await _collectionRepository.CreateAsync(defaultCollection);
                }

                // push
                var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId);
                await _pushRegistrationService.AddUserRegistrationOrganizationAsync(deviceIds,
                    organization.Id.ToString());
                await _pushNotificationService.PushSyncOrgKeysAsync(ownerId);

                return new Tuple<Organization, OrganizationMembershipProfile>(organization, orgUser);
            }
            catch
            {
                if (organization.Id != default(Guid))
                {
                    await _organizationRepository.DeleteAsync(organization);
                    await _applicationCacheService.DeleteOrganizationAbilityAsync(organization.Id);
                }

                throw;
            }
        }

        public async Task DeleteAsync(Organization organization)
        {
            if (!string.IsNullOrWhiteSpace(organization.GatewaySubscriptionId))
            {
                try
                {
                    var eop = !organization.ExpirationDate.HasValue ||
                        organization.ExpirationDate.Value >= DateTime.UtcNow;
                    await _referenceEventService.RaiseEventAsync(
                        new ReferenceEvent(ReferenceEventType.DeleteAccount, organization));
                }
                catch (GatewayException) { }
            }

            await _organizationRepository.DeleteAsync(organization);
            await _applicationCacheService.DeleteOrganizationAbilityAsync(organization.Id);
        }

        public async Task UpdateAsync(Organization organization)
        {
            if (organization.Id == default(Guid))
            {
                throw new ApplicationException("Cannot create org this way. Call SignUpAsync.");
            }

            if (!string.IsNullOrWhiteSpace(organization.Identifier))
            {
                var orgById = await _organizationRepository.GetByIdentifierAsync(organization.Identifier);
                if (orgById != null && orgById.Id != organization.Id)
                {
                    throw new BadRequestException("Identifier already in use by another organization.");
                }
            }

            await ReplaceAndUpdateCache(organization, EventType.Organization_Updated);
        }
        public async Task<List<OrganizationMembershipProfile>> InviteUserAsync(Guid organizationId,
            OrganizationUserInvite invite)
        {
            var organization = await GetOrgById(organizationId);
            if (organization == null || invite?.Emails == null)
            {
                throw new NotFoundException();
            }

            var orgUsers = new List<OrganizationMembershipProfile>();
            var orgUserInvitedCount = 0;
            foreach (var email in invite.Emails)
            {
                // Make sure user is not already invited

                if (await _organizationUserRepository.Any(x => x.OrganizationId == organizationId && x.Email == email))
                    continue;

                var u = await _userRepository.GetByEmailAsync(email);
                if (u == null)
                    continue;

                var orgUser = new OrganizationMembershipProfile
                {
                    OrganizationId = organizationId,
                    UserId = u.Id,
                    Email = email.ToLowerInvariant(),
                    Key = null,
                    Type = invite.Type,
                    Status = OrganizationUserStatusType.Invited,
                    AccessAll = invite.AccessAll,
                };

                await _organizationUserRepository.CreateAsync(orgUser);
                await SendInviteAsync(orgUser, organization);
                await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Invited);
                orgUsers.Add(orgUser);
                orgUserInvitedCount++;
            }
            await _referenceEventService.RaiseEventAsync(
                new ReferenceEvent(ReferenceEventType.InvitedUsers, organization)
                {
                    Users = orgUserInvitedCount
                });

            return orgUsers;
        }

        public async Task ResendInviteAsync(OrganizationMembershipProfile orgUser)
        {
            var membership = _sessionContext.GetMembership(orgUser.OrganizationId);
            if (membership == null || orgUser.Status != OrganizationUserStatusType.Invited)
                throw new BadRequestException("User invalid.");

            var org = await GetOrgById(orgUser.OrganizationId);
            await SendInviteAsync(orgUser, org);
        }

        private async Task SendInviteAsync(OrganizationMembershipProfile orgUser, Organization organization)
        {
            var nowMillis = CoreHelpers.ToEpocMilliseconds(DateTime.UtcNow);
            var token = _dataProtector.Protect(
                $"OrganizationUserInvite {orgUser.Id} {orgUser.Email} {nowMillis}");
            await _mailService.SendOrganizationInviteEmailAsync(organization.Name, orgUser, token);
        }

        public async Task<OrganizationMembershipProfile> AcceptUserAsync(OrganizationMembershipProfile orgUser, string token)
        {
            var user = await _userRepository.GetByIdAsync(orgUser.UserId);
            if (!CoreHelpers.UserInviteTokenIsValid(_dataProtector, token, user.Email, orgUser.Id, _globalSettings))
            {
                throw new BadRequestException("Invalid token.");
            }

            if (string.IsNullOrWhiteSpace(orgUser.Email) ||
                !orgUser.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException("User email does not match invite.");
            }

            if (orgUser.Status == OrganizationUserStatusType.Accepted)
            {
                throw new BadRequestException("You are already part of this organization.");
            }

            if (orgUser.Status != OrganizationUserStatusType.Invited)
            {
                throw new BadRequestException("Already accepted.");
            }

            orgUser.Status = OrganizationUserStatusType.Accepted;
            orgUser.UserId = user.Id;
            orgUser.Email = null;

            await _organizationUserRepository.ReplaceAsync(orgUser);

            // TODO: send notification emails to org admins and accepting user?
            return orgUser;
        }

        public async Task<OrganizationMembershipProfile> ConfirmUserAsync(OrganizationMembershipProfile orgUser, string key)
        {
            var membership = _sessionContext.GetMembership(orgUser.OrganizationId);
            if (orgUser == null || orgUser.Status != OrganizationUserStatusType.Accepted ||
                membership == null)
            {
                throw new BadRequestException("User not valid.");
            }

            var org = await GetOrgById(orgUser.OrganizationId);
            var user = await _userRepository.GetByIdAsync(orgUser.UserId);

            orgUser.Status = OrganizationUserStatusType.Confirmed;
            orgUser.Key = key;
            orgUser.Email = null;
            await _organizationUserRepository.ReplaceAsync(orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Confirmed);
            await _mailService.SendOrganizationConfirmedEmailAsync(org.Name, user.Email);

            // push
            var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId);
            await _pushRegistrationService.AddUserRegistrationOrganizationAsync(deviceIds, orgUser.OrganizationId.ToString());
            await _pushNotificationService.PushSyncOrgKeysAsync(orgUser.UserId);

            return orgUser;
        }

        public async Task SaveUserAsync(OrganizationMembershipProfile user)
        {
            var originalUser = await _organizationUserRepository.GetByIdAsync(user.Id);
            var confirmedOwners = (await GetConfirmedOwnersAsync(user.OrganizationId)).ToList();
            if (user.Type != OrganizationUserType.Owner &&
                confirmedOwners.Count == 1 && confirmedOwners[0].Id == user.Id)
            {
                throw new BadRequestException("Organization must have at least one confirmed owner.");
            }

            await _organizationUserRepository.ReplaceAsync(user);
            await _eventService.LogOrganizationUserEventAsync(user, EventType.OrganizationUser_Updated);
        }

        public async Task DeleteUserAsync(OrganizationMembershipProfile orgUser)
        {
            var organizationId = orgUser.OrganizationId;
            if (orgUser == null || orgUser.OrganizationId != organizationId)
            {
                throw new BadRequestException("User not valid.");
            }

            if (orgUser.UserId == _sessionContext.UserId)
            {
                throw new BadRequestException("You cannot remove yourself.");
            }

            if (orgUser.Type == OrganizationUserType.Owner)
            {
                if (_sessionContext.GetMembership(organizationId).Type != OrganizationUserType.Owner)
                {
                    throw new BadRequestException("Only owners can delete other owners.");
                }
            }

            await _organizationUserRepository.DeleteAsync(orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Removed);

            // push
            var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId);
            await _pushRegistrationService.DeleteUserRegistrationOrganizationAsync(deviceIds, organizationId.ToString());
            await _pushNotificationService.PushSyncOrgKeysAsync(orgUser.UserId);
        }


        public async Task RotateApiKeyAsync(Organization organization)
        {
            organization.ApiKey = CoreHelpers.SecureRandomString(30);
            await ReplaceAndUpdateCache(organization);
        }

        private async Task<IEnumerable<OrganizationMembershipProfile>> GetConfirmedOwnersAsync(Guid organizationId)
        {
            var owners = await _organizationUserRepository.GetManyByOrganizationAsync(organizationId,
                OrganizationUserType.Owner);
            return owners.Where(o => o.Status == OrganizationUserStatusType.Confirmed);
        }

        private async Task<IEnumerable<string>> GetUserDeviceIdsAsync(Guid userId)
        {
            var devices = await _deviceRepository.GetManyByUserIdAsync(userId);
            return devices.Where(d => !string.IsNullOrWhiteSpace(d.PushToken)).Select(d => d.Id.ToString());
        }

        private async Task ReplaceAndUpdateCache(Organization org, EventType? orgEvent = null)
        {
            await _organizationRepository.ReplaceAsync(org);
            await _applicationCacheService.UpsertOrganizationAbilityAsync(org);

            if (orgEvent.HasValue)
            {
                await _eventService.LogOrganizationEventAsync(org, orgEvent.Value);
            }
        }

        private async Task<Organization> GetOrgById(Guid id)
        {
            return await _organizationRepository.GetByIdAsync(id);
        }
    }
}
