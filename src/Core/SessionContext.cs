using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models.Table;
using Bit.Core.Enums;
using Microsoft.AspNetCore.Http;
using Bit.Core.Repositories;
using System.Threading.Tasks;
using System.Security.Claims;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;

namespace Bit.Core
{
    public interface ISessionContext
    {
        Guid UserId { get; set; }
        //public virtual User User { get; set; }
        string DeviceIdentifier { get; set; }
        DeviceType? DeviceType { get; set; }
        string IpAddress { get; set; }
        Guid? InstallationId { get; set; }
        Guid? OrganizationId { get; set; }
        List<OrganizationMembership> Organizations { get; set; }

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

    public class OrganizationMembership
    {
        public OrganizationMembership() { }

        public OrganizationMembership(OrganizationUser orgUser)
        {
            Id = orgUser.OrganizationId;
            Type = orgUser.Type;
            Permissions = CoreHelpers.LoadClassFromJsonData<Permissions>(orgUser.Permissions);
        }

        public Guid Id { get; set; }
        public OrganizationUserType Type { get; set; }

        public Permissions Permissions { get; set; }
    }
    // need better name.. this is the wrapper for claims.
    public class SessionContext : ISessionContext
    {
        public SessionContext(IHttpContextAccessor httpContextAccessor, GlobalSettings settings)
        {
            if (httpContextAccessor.HttpContext == null){
                Serilog.Log.Warning("sessionContext creation failed");    
                return;
            }

            if (httpContextAccessor.HttpContext.User == null){
                Serilog.Log.Warning("sessionContext user is null");    
                return;
            }
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new Exception("SessionContext can't get called before authentication. clean up wrong dependency");
            
            Build(httpContextAccessor.HttpContext, settings);            
        }

        // debug : cherry
        public virtual Dictionary<string, IEnumerable<string>> claims { get; set; }

        public virtual Guid UserId { get; set; }
        //public virtual User User { get; set; }
        public virtual string DeviceIdentifier { get; set; }
        public virtual DeviceType? DeviceType { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual List<OrganizationMembership> Organizations { get; set; }
        public virtual Guid? InstallationId { get; set; }
        public virtual Guid? OrganizationId { get; set; }

        public bool HasOrganizations() => Organizations?.Any() ?? false;

        private void Build(HttpContext httpContext, GlobalSettings globalSettings)
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

            IpAddress = httpContext.GetIpAddress(globalSettings);
            var user = httpContext.User;

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

            Organizations = new List<OrganizationMembership>();
            if (claims.ContainsKey("orgowner"))
            {
                Organizations.AddRange(claims["orgowner"].Select(c =>
                    new OrganizationMembership
                    {
                        Id = new Guid(c),
                        Type = OrganizationUserType.Owner
                    }));
            }
            else if (orgApi && OrganizationId.HasValue)
            {
                Organizations.Add(new OrganizationMembership
                {
                    Id = OrganizationId.Value,
                    Type = OrganizationUserType.Owner
                });
            }

            if (claims.ContainsKey("orgadmin"))
            {
                Organizations.AddRange(claims["orgadmin"].Select(c =>
                    new OrganizationMembership
                    {
                        Id = new Guid(c),
                        Type = OrganizationUserType.Admin
                    }));
            }

            if (claims.ContainsKey("orguser"))
            {
                Organizations.AddRange(claims["orguser"].Select(c =>
                    new OrganizationMembership
                    {
                        Id = new Guid(c),
                        Type = OrganizationUserType.User
                    }));
            }

            if (claims.ContainsKey("orgmanager"))
            {
                Organizations.AddRange(claims["orgmanager"].Select(c =>
                    new OrganizationMembership
                    {
                        Id = new Guid(c),
                        Type = OrganizationUserType.Manager
                    }));
            }

            if (claims.ContainsKey("orgcustom"))
            {
                Organizations.AddRange(claims["orgcustom"].Select(c =>
                    new OrganizationMembership
                    {
                        Id = new Guid(c),
                        Type = OrganizationUserType.Custom,
                        Permissions = SetOrganizationPermissionsFromClaims(c, claims)
                    }));
            }
        }

        public bool IsOrganizationMember(Guid orgId)
        {
            return Organizations?.Any(o => o.Id == orgId) ?? false;
        }

        public bool CanManageOrganization(Guid orgId)
        {
            return Organizations?.Any(o => o.Id == orgId &&
                (o.Type == OrganizationUserType.Owner || o.Type == OrganizationUserType.Admin ||
                    o.Type == OrganizationUserType.Manager)) ?? false;
        }

        public bool HasOrganizationAdminAccess(Guid orgId)
        {
            return Organizations?.Any(o => o.Id == orgId &&
                (o.Type == OrganizationUserType.Owner || o.Type == OrganizationUserType.Admin)) ?? false;
        }

        public bool IsOrganizationOwner(Guid orgId)
        {
            return Organizations?.Any(o => o.Id == orgId && o.Type == OrganizationUserType.Owner) ?? false;
        }

        public bool OrganizationCustom(Guid orgId)
        {
            return Organizations?.Any(o => o.Id == orgId && o.Type == OrganizationUserType.Custom) ?? false;
        }

        public bool AccessBusinessPortal(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.AccessBusinessPortal ?? false)) ?? false);
        }

        public bool AccessEventLogs(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.AccessEventLogs ?? false)) ?? false);
        }

        public bool AccessImportExport(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.AccessImportExport ?? false)) ?? false);
        }

        public bool AccessReports(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.AccessReports ?? false)) ?? false);
        }

        public bool ManageAllCollections(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManageAllCollections ?? false)) ?? false);
        }

        public bool ManageAssignedCollections(Guid orgId)
        {
            return CanManageOrganization(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManageAssignedCollections ?? false)) ?? false);
        }

        public bool ManageGroups(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManageGroups ?? false)) ?? false);
        }

        public bool ManagePolicies(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManagePolicies ?? false)) ?? false);
        }

        public bool ManageSso(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManageSso ?? false)) ?? false);
        }

        public bool ManageUsers(Guid orgId)
        {
            return HasOrganizationAdminAccess(orgId) || (Organizations?.Any(o => o.Id == orgId
                        && (o.Permissions?.ManageUsers ?? false)) ?? false);
        }

        private string GetClaimValue(Dictionary<string, IEnumerable<string>> claims, string type)
        {
            if (!claims.ContainsKey(type))
            {
                return null;
            }

            return claims[type].FirstOrDefault();
        }

        private Permissions SetOrganizationPermissionsFromClaims(string organizationId, Dictionary<string, IEnumerable<string>> claimsDict)
        {
            bool hasClaim(string claimKey)
            {
                return claimsDict.ContainsKey(claimKey) ?
                    claimsDict[claimKey].Any(x => x == organizationId) : false;
            }

            return new Permissions
            {
                AccessBusinessPortal = hasClaim("accessbusinessportal"),
                AccessEventLogs = hasClaim("accesseventlogs"),
                AccessImportExport = hasClaim("accessimportexport"),
                AccessReports = hasClaim("accessreports"),
                ManageAllCollections = hasClaim("manageallcollections"),
                ManageAssignedCollections = hasClaim("manageassignedcollections"),
                ManageGroups = hasClaim("managegroups"),
                ManagePolicies = hasClaim("managepolicies"),
                ManageSso = hasClaim("managesso"),
                ManageUsers = hasClaim("manageusers")
            };
        }


    }
}
