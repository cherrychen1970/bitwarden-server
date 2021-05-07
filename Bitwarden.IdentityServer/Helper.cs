
using System;
using System.Linq;
using System.Collections.Generic;
using IdentityModel;
using Bit.Core.Models;

namespace Bit.Core.IdentityServer
{
    public static class IdentityHelper
    {
        public static List<KeyValuePair<string, string>> BuildIdentityClaims(User user, ICollection<OrganizationMembership> orgs, bool isPremium) 

        {
            var claims = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("premium", isPremium ? "true" : "false"),
                new KeyValuePair<string, string>(JwtClaimTypes.Email, user.Email),
                new KeyValuePair<string, string>(JwtClaimTypes.EmailVerified, user.EmailVerified ? "true" : "false"),
                new KeyValuePair<string, string>("sstamp", user.SecurityStamp)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                claims.Add(new KeyValuePair<string, string>(JwtClaimTypes.Name, user.Name));
            }

            // Orgs that this user belongs to
            if (orgs.Any())
            {
                foreach (var group in orgs.GroupBy(o => o.Type))
                {
                    switch (group.Key)
                    {
                        case Enums.OrganizationUserType.Owner:
                            foreach (var org in group)
                            {
                                claims.Add(new KeyValuePair<string, string>("orgowner", org.OrganizationId.ToString()));
                            }
                            break;
                        case Enums.OrganizationUserType.Admin:
                            foreach (var org in group)
                            {
                                claims.Add(new KeyValuePair<string, string>("orgadmin", org.OrganizationId.ToString()));
                            }
                            break;
                        case Enums.OrganizationUserType.Manager:
                            foreach (var org in group)
                            {
                                claims.Add(new KeyValuePair<string, string>("orgmanager", org.OrganizationId.ToString()));
                            }
                            break;
                        case Enums.OrganizationUserType.User:
                            foreach (var org in group)
                            {
                                claims.Add(new KeyValuePair<string, string>("orguser", org.OrganizationId.ToString()));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return claims;
        }
    }
}

