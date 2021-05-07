using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;
using Bit.Core.Enums;

namespace Bit.Core.Entities
{
    public class OrganizationUser : DomainModels.IKey<Guid>, IEntityCreated,IEntityUpdated
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? UserId { get; set; }
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
            CreateMap<DomainModels.OrganizationUser, OrganizationUser>().ReverseMap();
            CreateMap<DomainModels.OrganizationUser, DomainModels.Data.OrganizationUserUserDetails>();
            CreateMap<DomainModels.OrganizationUser, DomainModels.Data.OrganizationUserOrganizationDetails>();
        }
    }
}
