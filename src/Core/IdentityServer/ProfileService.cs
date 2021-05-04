using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Bit.Core.Repositories;
using Bit.Core.Services;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using IdentityModel;
using Bit.Core.Models.Table;
using Bit.Core.Enums;
using Bit.Core.Utilities;

namespace Bit.Core.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly IUserService _userService;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly ILicensingService _licensingService;
        //private readonly CurrentContext _currentContext;
        private Guid _userId;

        public ProfileService(
            IUserService userService,
            IOrganizationUserRepository organizationUserRepository,
            ILicensingService licensingService
            )
        {
            _userService = userService;
            _organizationUserRepository = organizationUserRepository;
            _licensingService = licensingService;
            
            //_currentContext = currentContext;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var existingClaims = context.Subject.Claims;
            var newClaims = new List<Claim>();
            var userId = new Guid(context.Subject.FindFirstValue("sub"));            

            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                var isPremium = true;//await _licensingService.ValidateUserPremiumAsync(user);
                var orgs = await OrganizationMembershipAsync(_organizationUserRepository, user.Id);
                foreach (var claim in CoreHelpers.BuildIdentityClaims(user, orgs, isPremium))
                {
                    var upperValue = claim.Value.ToUpperInvariant();
                    var isBool = upperValue == "TRUE" || upperValue == "FALSE";
                    newClaims.Add(isBool ?
                        new Claim(claim.Key, claim.Value, ClaimValueTypes.Boolean) :
                        new Claim(claim.Key, claim.Value)
                    );
                }
            }

            // filter out any of the new claims
            var existingClaimsToKeep = existingClaims
                .Where(c => !c.Type.StartsWith("org") &&
                    (newClaims.Count == 0 || !newClaims.Any(nc => nc.Type == c.Type)))
                .ToList();

            newClaims.AddRange(existingClaimsToKeep);
            if (newClaims.Any())
            {
                context.IssuedClaims.AddRange(newClaims);
            }
        }

        public async Task<ICollection<OrganizationMembership>> OrganizationMembershipAsync(
            IOrganizationUserRepository organizationUserRepository, Guid userId)
        {
                var memberships= await organizationUserRepository.GetManyByUserAsync(userId);
                return memberships.Where(ou => ou.Status == OrganizationUserStatusType.Confirmed).Select(x=> new OrganizationMembership(x)).ToArray() ;                             
        }        

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var securityTokenClaim = context.Subject?.Claims.FirstOrDefault(c => c.Type == "sstamp");
            var userId = new Guid(context.Subject.FindFirstValue("sub"));            
            var user = await _userService.GetUserByIdAsync(userId);

            if (user != null && securityTokenClaim != null)
            {
                context.IsActive = string.Equals(user.SecurityStamp, securityTokenClaim.Value,
                    StringComparison.InvariantCultureIgnoreCase);
                return;
            }
            else
            {
                context.IsActive = true;
            }
        }
    }
}
