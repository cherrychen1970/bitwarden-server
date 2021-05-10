using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using AutoMapper;
using DomainModels = Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Models.Data;

namespace Bit.Core.Entities
{
    public class Cipher : DomainModels.IKey<Guid>, IEntityCreated, IEntityUpdated
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public CipherType Type { get; set; }
        public string Data { get; set; }
        //public CipherData Data { get; set; }
        //public string Favorites { get; set; }
        //public string Folders { get; set; }
        public Guid? FolderId { get; set; }
        public Guid? CollectionId { get; set; }
        public bool Favorite { get; set; }
        public string Attachments { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public DateTime? DeletedDate { get; set; }
        //public bool Edit { get; set; }=true;
        //public bool ViewPassword { get; set; }=true;        
        public Cipher()
        {
        }

        public User User { get; set; }
        public Organization Organization { get; set; }
        public Collection Collection { get; set; }
    }


    public class CipherMapperProfile : Profile
    {
        public CipherMapperProfile()
        {
            CreateMap<DomainModels.UserCipher, Cipher>()
                .Ignore(x => x.Id)
                .Ignore(x => x.Organization)
                .ForMember(x => x.Data, o => o.MapFrom(src => JsonConvert.SerializeObject(src.Data)))
            //.AfterMap((src,dest)=>dest.Data=  JsonConvert.SerializeObject(src.Data.ToString()))
            ;
            CreateMap<Cipher, DomainModels.UserCipher>()
                .ForMember(x => x.Data, o => o.MapFrom(src => JsonConvert.DeserializeObject<CipherLoginData>(src.Data)));
            //.AfterMap((src,dest)=>dest.Data=  JsonConvert.DeserializeObject<CipherLoginData>(src.Data));
            CreateMap<DomainModels.OrganizationCipher, Cipher>()
                .Ignore(x => x.Id)
                .ForMember(x => x.Organization, o => o.Ignore())
                .ForMember(x => x.Data, o => o.MapFrom(src => JsonConvert.SerializeObject(src.Data)))
            //.AfterMap((src,dest)=>dest.Data=  JsonConvert.SerializeObject(src.Data.ToString()))
            ;
            CreateMap<Cipher, DomainModels.OrganizationCipher>()
                .ForMember(x => x.Data, o => o.MapFrom(src => JsonConvert.DeserializeObject<CipherLoginData>(src.Data)));
            //.AfterMap((src,dest)=>dest.Data=  JsonConvert.DeserializeObject<CipherLoginData>(src.Data));

            // FIX
            CreateMap<DomainModels.Grant, DomainModels.Grant>();
            CreateMap<DomainModels.Device, DomainModels.Device>();
        }
    }
}
