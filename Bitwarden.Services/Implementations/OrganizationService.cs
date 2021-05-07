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
        private readonly IGroupRepository _groupRepository;
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
        //private readonly ITaxRateRepository _taxRateRepository;

        public OrganizationService(
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            ICollectionRepository collectionRepository,
            IUserRepository userRepository,
            IGroupRepository groupRepository,
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
            IMapper mapper
            //ITaxRateRepository taxRateRepository
            )
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
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
            //_taxRateRepository = taxRateRepository;
        }

        public async Task CancelSubscriptionAsync(Guid organizationId, bool? endOfPeriod = null)
        {
            var organization = await GetOrgById(organizationId);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            var eop = endOfPeriod.GetValueOrDefault(true);
            if (!endOfPeriod.HasValue && organization.ExpirationDate.HasValue &&
                organization.ExpirationDate.Value < DateTime.UtcNow)
            {
                eop = false;
            }
            
            await _referenceEventService.RaiseEventAsync(
                new ReferenceEvent(ReferenceEventType.CancelSubscription, organization)
                {
                    EndOfPeriod = endOfPeriod,
                });
        }

        public async Task ReinstateSubscriptionAsync(Guid organizationId)
        {
            var organization = await GetOrgById(organizationId);
            if (organization == null)
            {
                throw new NotFoundException();
            }
            
            await _referenceEventService.RaiseEventAsync(
                new ReferenceEvent(ReferenceEventType.ReinstateSubscription, organization));
        }


        public async Task<Tuple<Organization, OrganizationUser>> SignUpAsync(OrganizationSignup signup)
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
                Seats = (short)(plan.BaseSeats ),
                MaxCollections = plan.MaxCollections,
                MaxStorageGb = !plan.BaseStorageGb.HasValue ?
                    (short?)null : (short)(plan.BaseStorageGb.Value ),
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

            private async Task<Tuple<Organization, OrganizationUser>> SignUpAsync(Organization organization,
            Guid ownerId, string ownerKey, string collectionName, bool withPayment)
        {
            try
            {
                await _organizationRepository.CreateAsync(organization);
                await _applicationCacheService.UpsertOrganizationAbilityAsync(organization);

                var orgUser = new OrganizationUser
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
                var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId.Value);
                await _pushRegistrationService.AddUserRegistrationOrganizationAsync(deviceIds,
                    organization.Id.ToString());
                await _pushNotificationService.PushSyncOrgKeysAsync(ownerId);

                return new Tuple<Organization, OrganizationUser>(organization, orgUser);
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

        public async Task EnableAsync(Guid organizationId, DateTime? expirationDate)
        {
            var org = await GetOrgById(organizationId);
            if (org != null && !org.Enabled && org.Gateway.HasValue)
            {
                org.Enabled = true;
                org.ExpirationDate = expirationDate;
                await ReplaceAndUpdateCache(org);
            }
        }

        public async Task DisableAsync(Guid organizationId, DateTime? expirationDate)
        {
            var org = await GetOrgById(organizationId);
            if (org != null && org.Enabled)
            {
                org.Enabled = false;
                org.ExpirationDate = expirationDate;
                await ReplaceAndUpdateCache(org);

                // TODO: send email to owners?
            }
        }


        public async Task EnableAsync(Guid organizationId)
        {
            var org = await GetOrgById(organizationId);
            if (org != null && !org.Enabled)
            {
                org.Enabled = true;
                await ReplaceAndUpdateCache(org);
            }
        }

        public async Task UpdateAsync(Organization organization, bool updateBilling = false)
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

            if (updateBilling && !string.IsNullOrWhiteSpace(organization.GatewayCustomerId))
            {
                var customerService = new CustomerService();
                await customerService.UpdateAsync(organization.GatewayCustomerId, new CustomerUpdateOptions
                {
                    Email = organization.BillingEmail,
                    Description = organization.BusinessName
                });
            }
        }

        public async Task UpdateTwoFactorProviderAsync(Organization organization, TwoFactorProviderType type)
        {
            if (!type.ToString().Contains("Organization"))
            {
                throw new ArgumentException("Not an organization provider type.");
            }

            if (!organization.Use2fa)
            {
                throw new BadRequestException("Organization cannot use 2FA.");
            }

            var providers = organization.GetTwoFactorProviders();
            if (!providers?.ContainsKey(type) ?? true)
            {
                return;
            }

            providers[type].Enabled = true;
            organization.SetTwoFactorProviders(providers);
            await UpdateAsync(organization);
        }

        public async Task DisableTwoFactorProviderAsync(Organization organization, TwoFactorProviderType type)
        {
            if (!type.ToString().Contains("Organization"))
            {
                throw new ArgumentException("Not an organization provider type.");
            }

            var providers = organization.GetTwoFactorProviders();
            if (!providers?.ContainsKey(type) ?? true)
            {
                return;
            }

            providers.Remove(type);
            organization.SetTwoFactorProviders(providers);
            await UpdateAsync(organization);
        }

        public async Task<List<OrganizationUser>> InviteUserAsync(Guid organizationId, Guid? invitingUserId,
            string externalId, OrganizationUserInvite invite)
        {
            var organization = await GetOrgById(organizationId);
            if (organization == null || invite?.Emails == null)
            {
                throw new NotFoundException();
            }

            var orgUsers = new List<OrganizationUser>();
            var orgUserInvitedCount = 0;
            foreach (var email in invite.Emails)
            {
                // Make sure user is not already invited
                var existingOrgUserCount = await _organizationUserRepository.GetCountByOrganizationAsync(
                    organizationId, email, false);
                if (existingOrgUserCount > 0)
                {
                    continue;
                }

                var orgUser = new OrganizationUser
                {
                    OrganizationId = organizationId,
                    UserId = null,
                    Email = email.ToLowerInvariant(),
                    Key = null,
                    Type = invite.Type.Value,
                    Status = OrganizationUserStatusType.Invited,
                    AccessAll = invite.AccessAll,
                    ExternalId = externalId,
                };

                if (invite.Permissions != null)
                {
                    orgUser.Permissions = System.Text.Json.JsonSerializer.Serialize(invite.Permissions, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
                }

                if (!orgUser.AccessAll && invite.Collections.Any())
                {
                    await _organizationUserRepository.CreateAsync(orgUser, invite.Collections);
                }
                else
                {
                    await _organizationUserRepository.CreateAsync(orgUser);
                }

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

        public async Task ResendInviteAsync(Guid organizationId, Guid? invitingUserId, Guid organizationUserId)
        {
            var orgUser = await _organizationUserRepository.GetByIdAsync(organizationUserId);
            if (orgUser == null || orgUser.OrganizationId != organizationId ||
                orgUser.Status != OrganizationUserStatusType.Invited)
            {
                throw new BadRequestException("User invalid.");
            }

            var org = await GetOrgById(orgUser.OrganizationId);
            await SendInviteAsync(orgUser, org);
        }

        private async Task SendInviteAsync(OrganizationUser orgUser, Organization organization)
        {
            var nowMillis = CoreHelpers.ToEpocMilliseconds(DateTime.UtcNow);
            var token = _dataProtector.Protect(
                $"OrganizationUserInvite {orgUser.Id} {orgUser.Email} {nowMillis}");
            await _mailService.SendOrganizationInviteEmailAsync(organization.Name, orgUser, token);
        }

        public async Task<OrganizationUser> AcceptUserAsync(Guid organizationUserId, User user, string token,
            IUserService userService)
        {
            var orgUser = await _organizationUserRepository.GetByIdAsync(organizationUserId);
            if (orgUser == null)
            {
                throw new BadRequestException("User invalid.");
            }

            if (!CoreHelpers.UserInviteTokenIsValid(_dataProtector, token, user.Email, orgUser.Id, _globalSettings))
            {
                throw new BadRequestException("Invalid token.");
            }

            if (string.IsNullOrWhiteSpace(orgUser.Email) ||
                !orgUser.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException("User email does not match invite.");
            }

            var existingOrgUserCount = await _organizationUserRepository.GetCountByOrganizationAsync(
                orgUser.OrganizationId, user.Email, true);
            if (existingOrgUserCount > 0)
            {
                throw new BadRequestException("You are already part of this organization.");
            }

            return await AcceptUserAsync(orgUser, user, userService);
        }

        public async Task<OrganizationUser> AcceptUserAsync(Guid organizationId, User user, IUserService userService)
        {
            var org = await _organizationRepository.GetByIdAsync(organizationId);
            if (org == null)
            {
                throw new BadRequestException("Organization invalid.");
            }

            var usersOrgs = await _organizationUserRepository.GetManyByUserAsync(user.Id);
            var orgUser = usersOrgs.FirstOrDefault(u => u.OrganizationId == org.Id);
            if (orgUser == null)
            {
                throw new BadRequestException("User not found within organization.");
            }

            return await AcceptUserAsync(orgUser, user, userService);
        }

        private async Task<OrganizationUser> AcceptUserAsync(OrganizationUser orgUser, User user,
            IUserService userService)
        {
            if (orgUser.Status != OrganizationUserStatusType.Invited)
            {
                throw new BadRequestException("Already accepted.");
            }

            ICollection<Policy> orgPolicies = null;
            ICollection<Policy> userPolicies = null;
            async Task<bool> hasPolicyAsync(PolicyType policyType, bool useUserPolicies = false)
            {
                var policies = useUserPolicies ?
                    userPolicies = userPolicies ?? await _policyRepository.GetManyByUserIdAsync(user.Id) :
                    orgPolicies = orgPolicies ?? await _policyRepository.GetManyByOrganizationIdAsync(orgUser.OrganizationId);

                return policies.Any(p => p.Type == policyType && p.Enabled);
            }
            var userOrgs = await _organizationUserRepository.GetManyByUserAsync(user.Id);
            if (userOrgs.Any(ou => ou.OrganizationId != orgUser.OrganizationId && ou.Status != OrganizationUserStatusType.Invited))
            {
                if (await hasPolicyAsync(PolicyType.SingleOrg))
                {
                    throw new BadRequestException("You may not join this organization until you leave or remove " +
                        "all other organizations.");
                }
                if (await hasPolicyAsync(PolicyType.SingleOrg, true))
                {
                    throw new BadRequestException("You cannot join this organization because you are a member of " +
                        "an organization which forbids it");
                }
            }

            if (!await userService.TwoFactorIsEnabledAsync(user))
            {
                if (await hasPolicyAsync(PolicyType.TwoFactorAuthentication))
                {
                    throw new BadRequestException("You cannot join this organization until you enable " +
                        "two-step login on your user account.");
                }
            }

            orgUser.Status = OrganizationUserStatusType.Accepted;
            orgUser.UserId = user.Id;
            orgUser.Email = null;

            await _organizationUserRepository.ReplaceAsync(orgUser);

            // TODO: send notification emails to org admins and accepting user?
            return orgUser;
        }

        public async Task<OrganizationUser> ConfirmUserAsync(Guid organizationId, Guid organizationUserId, string key,
            Guid confirmingUserId, IUserService userService)
        {
            var orgUser = await _organizationUserRepository.GetByIdAsync(organizationUserId);
            if (orgUser == null || orgUser.Status != OrganizationUserStatusType.Accepted ||
                orgUser.OrganizationId != organizationId)
            {
                throw new BadRequestException("User not valid.");
            }

            var org = await GetOrgById(organizationId);
            if (org.PlanType == PlanType.Free &&
                (orgUser.Type == OrganizationUserType.Admin || orgUser.Type == OrganizationUserType.Owner))
            {
                var adminCount = await _organizationUserRepository.GetCountByFreeOrganizationAdminUserAsync(
                    orgUser.UserId.Value);
                if (adminCount > 0)
                {
                    throw new BadRequestException("User can only be an admin of one free organization.");
                }
            }

            var user = await _userRepository.GetByIdAsync(orgUser.UserId.Value);
            var policies = await _policyRepository.GetManyByOrganizationIdAsync(organizationId);
            var usingTwoFactorPolicy = policies.Any(p => p.Type == PolicyType.TwoFactorAuthentication && p.Enabled);
            if (usingTwoFactorPolicy && !(await userService.TwoFactorIsEnabledAsync(user)))
            {
                throw new BadRequestException("User does not have two-step login enabled.");
            }

            var usingSingleOrgPolicy = policies.Any(p => p.Type == PolicyType.SingleOrg && p.Enabled);
            if (usingSingleOrgPolicy)
            {
                var userOrgs = await _organizationUserRepository.GetManyByUserAsync(user.Id);
                if (userOrgs.Any(ou => ou.OrganizationId != organizationId && ou.Status != OrganizationUserStatusType.Invited))
                {
                    throw new BadRequestException("User is a member of another organization.");
                }
            }

            orgUser.Status = OrganizationUserStatusType.Confirmed;
            orgUser.Key = key;
            orgUser.Email = null;
            await _organizationUserRepository.ReplaceAsync(orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Confirmed);
            await _mailService.SendOrganizationConfirmedEmailAsync(org.Name, user.Email);

            // push
            var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId.Value);
            await _pushRegistrationService.AddUserRegistrationOrganizationAsync(deviceIds, organizationId.ToString());
            await _pushNotificationService.PushSyncOrgKeysAsync(orgUser.UserId.Value);

            return orgUser;
        }

        public async Task SaveUserAsync(OrganizationUser user, Guid? savingUserId,
            IEnumerable<SelectionReadOnly> collections)
        {
            if (user.Id.Equals(default(Guid)))
            {
                throw new BadRequestException("Invite the user first.");
            }

            var originalUser = await _organizationUserRepository.GetByIdAsync(user.Id);
            if (user.Equals(originalUser))
            {
                throw new BadRequestException("Please make changes before saving.");
            }

            var confirmedOwners = (await GetConfirmedOwnersAsync(user.OrganizationId)).ToList();
            if (user.Type != OrganizationUserType.Owner &&
                confirmedOwners.Count == 1 && confirmedOwners[0].Id == user.Id)
            {
                throw new BadRequestException("Organization must have at least one confirmed owner.");
            }

            if (user.AccessAll)
            {
                // We don't need any collections if we're flagged to have all access.
                collections = new List<SelectionReadOnly>();
            }
            await _organizationUserRepository.ReplaceAsync(user, collections);
            await _eventService.LogOrganizationUserEventAsync(user, EventType.OrganizationUser_Updated);
        }

        public async Task DeleteUserAsync(Guid organizationId, Guid organizationUserId, Guid? deletingUserId)
        {
            var orgUser = await _organizationUserRepository.GetByIdAsync(organizationUserId);
            if (orgUser == null || orgUser.OrganizationId != organizationId)
            {
                throw new BadRequestException("User not valid.");
            }

            if (deletingUserId.HasValue && orgUser.UserId == deletingUserId.Value)
            {
                throw new BadRequestException("You cannot remove yourself.");
            }

            if (orgUser.Type == OrganizationUserType.Owner && deletingUserId.HasValue)
            {
                var deletingUserOrgs = await _organizationUserRepository.GetManyByUserAsync(deletingUserId.Value);
                var anyOwners = deletingUserOrgs.Any(
                    u => u.OrganizationId == organizationId && u.Type == OrganizationUserType.Owner);
                if (!anyOwners)
                {
                    throw new BadRequestException("Only owners can delete other owners.");
                }
            }

            var confirmedOwners = (await GetConfirmedOwnersAsync(organizationId)).ToList();
            if (confirmedOwners.Count == 1 && confirmedOwners[0].Id == organizationUserId)
            {
                throw new BadRequestException("Organization must have at least one confirmed owner.");
            }

            await _organizationUserRepository.DeleteAsync(orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Removed);

            if (orgUser.UserId.HasValue)
            {
                // push
                var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId.Value);
                await _pushRegistrationService.DeleteUserRegistrationOrganizationAsync(deviceIds,
                    organizationId.ToString());
                await _pushNotificationService.PushSyncOrgKeysAsync(orgUser.UserId.Value);
            }
        }

        public async Task DeleteUserAsync(Guid organizationId, Guid userId)
        {
            var orgUser = await _organizationUserRepository.GetByOrganizationAsync(organizationId, userId);
            if (orgUser == null)
            {
                throw new NotFoundException();
            }

            var confirmedOwners = (await GetConfirmedOwnersAsync(organizationId)).ToList();
            if (confirmedOwners.Count == 1 && confirmedOwners[0].Id == orgUser.Id)
            {
                throw new BadRequestException("Organization must have at least one confirmed owner.");
            }

            await _organizationUserRepository.DeleteAsync(orgUser);
            await _eventService.LogOrganizationUserEventAsync(orgUser, EventType.OrganizationUser_Removed);

            if (orgUser.UserId.HasValue)
            {
                // push
                var deviceIds = await GetUserDeviceIdsAsync(orgUser.UserId.Value);
                await _pushRegistrationService.DeleteUserRegistrationOrganizationAsync(deviceIds,
                    organizationId.ToString());
                await _pushNotificationService.PushSyncOrgKeysAsync(orgUser.UserId.Value);
            }
        }

        public async Task UpdateUserGroupsAsync(OrganizationUser organizationUser, IEnumerable<Guid> groupIds, Guid? loggedInUserId)
        {
            await _organizationUserRepository.UpdateGroupsAsync(organizationUser.Id, groupIds);
            await _eventService.LogOrganizationUserEventAsync(organizationUser,
                EventType.OrganizationUser_UpdatedGroups);
        }

        public async Task ImportAsync(Guid organizationId,
            Guid? importingUserId,
            IEnumerable<ImportedGroup> groups,
            IEnumerable<ImportedOrganizationUser> newUsers,
            IEnumerable<string> removeUserExternalIds,
            bool overwriteExisting)
        {
            var organization = await GetOrgById(organizationId);
            if (organization == null)
            {
                throw new NotFoundException();
            }

            if (!organization.UseDirectory)
            {
                throw new BadRequestException("Organization cannot use directory syncing.");
            }

            var newUsersSet = new HashSet<string>(newUsers?.Select(u => u.ExternalId) ?? new List<string>());
            var existingUsers = await _organizationUserRepository.GetManyDetailsByOrganizationAsync(organizationId);
            var existingExternalUsers = existingUsers.Where(u => !string.IsNullOrWhiteSpace(u.ExternalId)).ToList();
            var existingExternalUsersIdDict = existingExternalUsers.ToDictionary(u => u.ExternalId, u => u.Id);

            // Users

            // Remove Users
            if (removeUserExternalIds?.Any() ?? false)
            {
                var removeUsersSet = new HashSet<string>(removeUserExternalIds);
                var existingUsersDict = existingExternalUsers.ToDictionary(u => u.ExternalId);

                var usersToRemove = removeUsersSet
                    .Except(newUsersSet)
                    .Where(ru => existingUsersDict.ContainsKey(ru))
                    .Select(ru => existingUsersDict[ru]);

                foreach (var user in usersToRemove)
                {
                    if (user.Type != OrganizationUserType.Owner)
                    {
                        await _organizationUserRepository.DeleteAsync(new OrganizationUser { Id = user.Id });
                        existingExternalUsersIdDict.Remove(user.ExternalId);
                    }
                }
            }

            if (overwriteExisting)
            {
                // Remove existing external users that are not in new user set
                foreach (var user in existingExternalUsers)
                {
                    if (user.Type != OrganizationUserType.Owner && !newUsersSet.Contains(user.ExternalId) &&
                        existingExternalUsersIdDict.ContainsKey(user.ExternalId))
                    {
                        await _organizationUserRepository.DeleteAsync(new OrganizationUser { Id = user.Id });
                        existingExternalUsersIdDict.Remove(user.ExternalId);
                    }
                }
            }

            if (newUsers?.Any() ?? false)
            {
                // Marry existing users
                var existingUsersEmailsDict = existingUsers
                    .Where(u => string.IsNullOrWhiteSpace(u.ExternalId))
                    .ToDictionary(u => u.Email);
                var newUsersEmailsDict = newUsers.ToDictionary(u => u.Email);
                var usersToAttach = existingUsersEmailsDict.Keys.Intersect(newUsersEmailsDict.Keys).ToList();
                foreach (var user in usersToAttach)
                {
                    var orgUserDetails = existingUsersEmailsDict[user];
                    var orgUser = await _organizationUserRepository.GetByIdAsync(orgUserDetails.Id);
                    if (orgUser != null)
                    {
                        orgUser.ExternalId = newUsersEmailsDict[user].ExternalId;
                        await _organizationUserRepository.UpsertAsync(orgUser);
                        existingExternalUsersIdDict.Add(orgUser.ExternalId, orgUser.Id);
                    }
                }

                // Add new users
                var existingUsersSet = new HashSet<string>(existingExternalUsersIdDict.Keys);
                var usersToAdd = newUsersSet.Except(existingUsersSet).ToList();

                var seatsAvailable = int.MaxValue;
                var enoughSeatsAvailable = true;
                if (organization.Seats.HasValue)
                {
                    var userCount = await _organizationUserRepository.GetCountByOrganizationIdAsync(organizationId);
                    seatsAvailable = organization.Seats.Value - userCount;
                    enoughSeatsAvailable = seatsAvailable >= usersToAdd.Count;
                }

                if (enoughSeatsAvailable)
                {
                    foreach (var user in newUsers)
                    {
                        if (!usersToAdd.Contains(user.ExternalId) || string.IsNullOrWhiteSpace(user.Email))
                        {
                            continue;
                        }

                        try
                        {
                            var invite = new OrganizationUserInvite
                            {
                                Emails = new List<string> { user.Email },
                                Type = OrganizationUserType.User,
                                AccessAll = false,
                                Collections = new List<SelectionReadOnly>(),
                            };
                            var newUserPromise = await InviteUserAsync(organizationId, importingUserId, user.ExternalId, invite);
                            var newUser = newUserPromise.FirstOrDefault();

                            existingExternalUsersIdDict.Add(newUser.ExternalId, newUser.Id);
                        }
                        catch (BadRequestException)
                        {
                            continue;
                        }
                    }
                }
            }

            // Groups

            if (groups?.Any() ?? false)
            {
                if (!organization.UseGroups)
                {
                    throw new BadRequestException("Organization cannot use groups.");
                }

                var groupsDict = groups.ToDictionary(g => g.Group.ExternalId);
                var existingGroups = await _groupRepository.GetManyByOrganizationIdAsync(organizationId);
                var existingExternalGroups = existingGroups
                    .Where(u => !string.IsNullOrWhiteSpace(u.ExternalId)).ToList();
                var existingExternalGroupsDict = existingExternalGroups.ToDictionary(g => g.ExternalId);

                var newGroups = groups
                    .Where(g => !existingExternalGroupsDict.ContainsKey(g.Group.ExternalId))
                    .Select(g => g.Group);

                foreach (var group in newGroups)
                {
                    await _groupRepository.CreateAsync(group);
                    await UpdateUsersAsync(group, groupsDict[group.ExternalId].ExternalUserIds,
                        existingExternalUsersIdDict);
                }

                var updateGroups = existingExternalGroups
                    .Where(g => groupsDict.ContainsKey(g.ExternalId))
                    .ToList();

                if (updateGroups.Any())
                {
                    var groupUsers = await _groupRepository.GetManyGroupUsersByOrganizationIdAsync(organizationId);
                    var existingGroupUsers = groupUsers
                        .GroupBy(gu => gu.GroupId)
                        .ToDictionary(g => g.Key, g => new HashSet<Guid>(g.Select(gr => gr.OrganizationUserId)));

                    foreach (var group in updateGroups)
                    {
                        var updatedGroup = groupsDict[group.ExternalId].Group;
                        if (group.Name != updatedGroup.Name)
                        {
                            group.RevisionDate = DateTime.UtcNow;
                            group.Name = updatedGroup.Name;

                            await _groupRepository.ReplaceAsync(group);
                        }

                        await UpdateUsersAsync(group, groupsDict[group.ExternalId].ExternalUserIds,
                            existingExternalUsersIdDict,
                            existingGroupUsers.ContainsKey(group.Id) ? existingGroupUsers[group.Id] : null);
                    }
                }
            }
        }

        public async Task RotateApiKeyAsync(Organization organization)
        {
            organization.ApiKey = CoreHelpers.SecureRandomString(30);
            await ReplaceAndUpdateCache(organization);
        }

        public async Task DeleteSsoUserAsync(Guid userId, Guid? organizationId)
        {
            await _ssoUserRepository.DeleteAsync(userId, organizationId);
            if (organizationId.HasValue)
            {
                var organizationUser = await _organizationUserRepository.GetByOrganizationAsync(organizationId.Value, userId);
                if (organizationUser != null)
                {
                    await _eventService.LogOrganizationUserEventAsync(organizationUser, EventType.OrganizationUser_UnlinkedSso);
                }
            }
        }

        private async Task UpdateUsersAsync(Group group, HashSet<string> groupUsers,
            Dictionary<string, Guid> existingUsersIdDict, HashSet<Guid> existingUsers = null)
        {
            var availableUsers = groupUsers.Intersect(existingUsersIdDict.Keys);
            var users = new HashSet<Guid>(availableUsers.Select(u => existingUsersIdDict[u]));
            if (existingUsers != null && existingUsers.Count == users.Count && users.SetEquals(existingUsers))
            {
                return;
            }

            await _groupRepository.UpdateUsersAsync(group.Id, users);
        }

        private async Task<IEnumerable<OrganizationUser>> GetConfirmedOwnersAsync(Guid organizationId)
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
