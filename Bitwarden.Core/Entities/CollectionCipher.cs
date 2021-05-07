using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;

namespace Bit.Core.Entities
{    
    public class CollectionCipher 
    {        
        virtual public int Id {get;set;}

        public Guid CollectionId { get; set; }
        public Guid CipherId { get; set; }        
        virtual public Collection Collection {get;set;}
        public Cipher Cipher { get; set; }
    }

    public class CollectionCipherProfile : Profile
    {
        public CollectionCipherProfile()
        {
            CreateMap<DomainModels.CollectionCipher, CollectionCipher>().ReverseMap();
        }
    }
}
