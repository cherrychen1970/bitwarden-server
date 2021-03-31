using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{    
    public class CollectionCipher : Table.CollectionCipher
    {        
        virtual public int Id {get;set;}
        virtual public Collection Collection {get;set;}
        public Cipher Cipher { get; set; }
    }

    public class CollectionCipherProfile : Profile
    {
        public CollectionCipherProfile()
        {
            CreateMap<Table.CollectionCipher, CollectionCipher>().ReverseMap();
        }
    }
}
