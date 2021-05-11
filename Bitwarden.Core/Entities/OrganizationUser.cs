using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;
using Bit.Core.Enums;

namespace Bit.Core.Entities
{
    public class OrganizationUser : BaseGuidEntity, IEntityCreated,IEntityUpdated
    {        
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public OrganizationUserStatusType Status { get; set; }
        public OrganizationUserType Type { get; set; }
        public bool AccessAll { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public string Permissions { get; set; }        
        virtual public Organization Organization {get;set;}
        virtual public User User {get;set;}
        //public ICollection<Cipher> Ciphers { get; set; }        
    }

    public class OrganizationUserProfile : Profile
    {
        public OrganizationUserProfile()
        {
            CreateMap<OrganizationUser,DomainModels.OrganizationMembershipProfile>();

            CreateMap<DomainModels.OrganizationMembershipProfile,OrganizationUser>()
                .Ignore(x=>x.Id);
               
            CreateMap<OrganizationUser, DomainModels.OrganizationMembership>();                            
        }
    }
}
