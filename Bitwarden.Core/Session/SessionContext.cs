using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Utilities;

namespace Bit.Core
{
    public interface ISessionContext
    {
        Guid UserId { get; set; }
        //public virtual User User { get; set; }
        Guid? InstallationId { get; set; }
        //Guid? OrganizationId { get; set; }
        List<OrganizationMembership> OrganizationMemberships { get; set; }
        OrganizationMembership GetMembership(Guid organizationId)
            => OrganizationMemberships.SingleOrDefault(x => x.OrganizationId == organizationId);

        bool HasOrganizations();
        bool ManageAllCollections(Guid orgId);
        bool ManageAssignedCollections(Guid orgId);
        bool ManageGroups(Guid orgId);
        bool ManageUsers(Guid orgId);
        bool ManagePolicies(Guid orgId);

        bool IsOrganizationMember(Guid orgId);
        //bool CanManageOrganization(Guid orgId);
        bool HasOrganizationAdminAccess(Guid orgId);
        bool IsOrganizationOwner(Guid orgId);

        bool AccessReports(Guid orgId);
        bool AccessImportExport(Guid orgId);
        bool AccessEventLogs(Guid orgId);
    }


    // need better name.. this is the wrapper for claims.
    public class SessionContext : ISessionContext
    {
        public SessionContext(IHttpContextAccessor httpContextAccessor)
        {
            /*
            if (httpContextAccessor.HttpContext == null)
            {
                Serilog.Log.Warning("sessionContext creation failed");
                return;
            }

            if (httpContextAccessor.HttpContext.User == null)
            {
                Serilog.Log.Warning("sessionContext user is null");
                return;
            }
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new Exception("SessionContext can't get called before authentication. clean up wrong dependency");
            */
            if (httpContextAccessor.HttpContext == null)
                return;
            if (httpContextAccessor.HttpContext.User == null)
                return;
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return;

            Build(httpContextAccessor.HttpContext);
        }

        public SessionContext(ClaimsPrincipal user)
        {
            /*
            if (httpContextAccessor.HttpContext == null)
            {
                Serilog.Log.Warning("sessionContext creation failed");
                return;
            }

            if (httpContextAccessor.HttpContext.User == null)
            {
                Serilog.Log.Warning("sessionContext user is null");
                return;
            }
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new Exception("SessionContext can't get called before authentication. clean up wrong dependency");
            */
            if (!user.Identity.IsAuthenticated)
                return;

            Build(user);
        }

        // debug : cherry
        public virtual Dictionary<string, IEnumerable<string>> claims { get; set; }

        public virtual Guid UserId { get; set; }
        //public virtual User User { get; set; }
        public virtual string DeviceIdentifier { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual List<OrganizationMembership> OrganizationMemberships { get; set; }
        public virtual Guid? InstallationId { get; set; }
        //public virtual Guid? OrganizationId { get; set; }

        public bool HasOrganizations() => OrganizationMemberships?.Any() ?? false;

        private void Build(HttpContext httpContext)
        {
            if (DeviceIdentifier == null && httpContext.Request.Headers.ContainsKey("Device-Identifier"))
            {
                DeviceIdentifier = httpContext.Request.Headers["Device-Identifier"];
            }

            if (httpContext.Request.Headers.ContainsKey("Device-Type") &&
                Enum.TryParse(httpContext.Request.Headers["Device-Type"].ToString(), out DeviceType dType))
            {
                DeviceType = dType;
            }

            IpAddress = httpContext.GetIpAddress();
            Build(httpContext.User);
        }

        private void Build(ClaimsPrincipal user)
        {
            if (user == null || !user.Claims.Any())
            {
                return;
            }

            claims = user.Claims.GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(v => v.Value));

            var subject = GetClaimValue(claims, "sub");
            if (Guid.TryParse(subject, out var subIdGuid))
            {
                UserId = subIdGuid;
            }

            var clientId = GetClaimValue(claims, "client_id");
            var clientSubject = GetClaimValue(claims, "client_sub");
            var orgApi = false;
            Guid? OrganizationId = null;
            if (clientSubject != null)
            {
                if (clientId?.StartsWith("installation.") ?? false)
                {
                    if (Guid.TryParse(clientSubject, out var idGuid))
                    {
                        InstallationId = idGuid;
                    }
                }
                else if (clientId?.StartsWith("organization.") ?? false)
                {
                    if (Guid.TryParse(clientSubject, out var idGuid))
                    {
                        OrganizationId = idGuid;
                        orgApi = true;
                    }
                }
            }

            DeviceIdentifier = GetClaimValue(claims, "device");

            OrganizationMemberships = new List<OrganizationMembership>();
            if (claims.ContainsKey("orgowner"))
            {
                OrganizationMemberships.AddRange(claims["orgowner"].Select(c => new OrganizationMembership(new Guid(c), UserId, OrganizationUserType.Owner)));
            }
            else if (orgApi && OrganizationId.HasValue)
            {
                OrganizationMemberships.AddRange(claims["orgowner"].Select(c => new OrganizationMembership(new Guid(c), UserId, OrganizationUserType.Owner)));
            }

            if (claims.ContainsKey("orgadmin"))
            {
                OrganizationMemberships.AddRange(claims["orgadmin"].Select(c => new OrganizationMembership(new Guid(c), UserId, OrganizationUserType.Admin)));
            }

            if (claims.ContainsKey("orguser"))
            {
                OrganizationMemberships.AddRange(claims["orguser"].Select(c => new OrganizationMembership(new Guid(c), UserId, OrganizationUserType.User)));
            }

            if (claims.ContainsKey("orgmanager"))
            {
                OrganizationMemberships.AddRange(claims["orgmanager"].Select(c => new OrganizationMembership(new Guid(c), UserId, OrganizationUserType.Manager)));
            }
        }

        public bool IsOrganizationMember(Guid orgId)
        {
            return OrganizationMemberships?.Any(o => o.OrganizationId == orgId) ?? false;
        }

        public bool CanManageOrganization(Guid orgId)
        {
            return OrganizationMemberships?.Any(o => o.OrganizationId == orgId &&
                (o.Type == OrganizationUserType.Owner || o.Type == OrganizationUserType.Admin ||
                    o.Type == OrganizationUserType.Manager)) ?? false;
        }

        public bool HasOrganizationAdminAccess(Guid orgId)
        {
            return OrganizationMemberships?.Any(o => o.OrganizationId == orgId &&
                (o.Type == OrganizationUserType.Owner || o.Type == OrganizationUserType.Admin)) ?? false;
        }

        public bool IsOrganizationOwner(Guid orgId)
        {
            return OrganizationMemberships?.Any(o => o.OrganizationId == orgId && o.Type == OrganizationUserType.Owner) ?? false;
        }

        public bool OrganizationCustom(Guid orgId)
        {
            return OrganizationMemberships?.Any(o => o.OrganizationId == orgId && o.Type == OrganizationUserType.Custom) ?? false;
        }

        public bool AccessBusinessPortal(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool AccessEventLogs(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool AccessImportExport(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool AccessReports(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool ManageAllCollections(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool ManageAssignedCollections(Guid orgId)
        {
            return CanManageOrganization(orgId);
        }

        public bool ManageGroups(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool ManagePolicies(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool ManageSso(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        public bool ManageUsers(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId);
        }

        private string GetClaimValue(Dictionary<string, IEnumerable<string>> claims, string type)
        {
            if (!claims.ContainsKey(type))
            {
                return null;
            }

            return claims[type].FirstOrDefault();
        }
    }
}
