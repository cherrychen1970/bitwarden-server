using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;

namespace Bit.Core.Entities
{
    public class Collection : DomainModels.IKey<Guid>, IEntityCreated,IEntityUpdated
    {        
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;        
        virtual public Organization Organization {get;set;}
        //public ICollection<Cipher> Ciphers { get; set; }
    }

    public class CollectionProfile : Profile
    {
        public CollectionProfile()
        {
            CreateMap<DomainModels.Collection, Collection>().ReverseMap();                        
            ;
        }
    }
}
