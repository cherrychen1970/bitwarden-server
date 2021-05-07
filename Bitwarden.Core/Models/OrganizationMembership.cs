using System;
using Bit.Core.Models;
using Bit.Core.Enums;

namespace Bit.Core.Models
{
    public class OrganizationMembership
    {        
        public OrganizationMembership(Guid orgId, Guid userId,OrganizationUserType type)     
        {        
         
        }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public OrganizationUserType Type { get; set; }
    }
}