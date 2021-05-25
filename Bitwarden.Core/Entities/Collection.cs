using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels = Bit.Core.Models;

namespace Bit.Core.Entities
{
    public class Collection : BaseGuidEntity , IEntityCreated, IEntityUpdated
    {        
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public bool AdminOnly {get;set;}
        public bool ReadOnly {get;set;}
        //public Enums.CollectionAccessType ReadAccess {get;set;} = Enums.CollectionAccessType.All;
        //public Enums.CollectionAccessType WriteAccess {get;set;} = Enums.CollectionAccessType.All;        
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        virtual public Organization Organization { get; set; }
        //public ICollection<Cipher> Ciphers { get; set; }
    }

    public class CollectionProfile : Profile
    {
        public CollectionProfile()
        {
            CreateMap<DomainModels.Collection, Collection>()
                .Ignore(x => x.Id);
            CreateMap<Collection, DomainModels.Collection>();



        }
    }
}
