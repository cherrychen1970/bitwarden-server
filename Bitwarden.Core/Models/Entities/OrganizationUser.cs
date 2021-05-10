using System;
using Bit.Core.Utilities;
using Bit.Core.Enums;

namespace Bit.Core.Models
{
    public class OrganizationMember1 : IKey<Guid>
    {
        public Guid Id { get; set; }//= Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string OrganizationName {get;set;}
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }

    public class OrganizationMembership
    {
        public OrganizationMembership()
        {}
        public OrganizationMembership(Guid orgId, Guid userId, OrganizationUserType type)
        {
            OrganizationId=orgId;
            UserId=userId;
            Type=type;
        }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public OrganizationUserType Type { get; set; }
    }
    public class OrganizationMembershipProfile : OrganizationMembership, IKey<Guid>, IExternal
    {
        public Guid Id { get; set; }//= Guid.NewGuid();
        //public Guid OrganizationId { get; set; }
        public string OrganizationName {get;set;}
        //public Guid UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Key { get; set; }
        public OrganizationUserStatusType Status { get; set; }
        //public OrganizationUserType Type { get; set; }
        public bool AccessAll { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public string Permissions { get; set; }
        public void SetNewId()=>Id=Guid.NewGuid();
    }
}
