using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{
    public class Collection : Table.Collection,IEntityCreated,IEntityUpdated
    {        
        virtual public Organization Organization {get;set;}
        //public ICollection<Cipher> Ciphers { get; set; }
    }

    public class CollectionProfile : Profile
    {
        public CollectionProfile()
        {
            CreateMap<Table.Collection, Collection>().ReverseMap();
            CreateMap<Collection,Data.CollectionDetails>()
            //.ForMember(x=>x.HidePasswords, y=>y.MapFrom(z=>z.or))
            ;
        }
    }
}
