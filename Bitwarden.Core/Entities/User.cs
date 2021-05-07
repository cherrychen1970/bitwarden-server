using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;

namespace Bit.Core.Entities
{
    public class User : DomainModels.User
    {
        private JsonDocument _twoFactorProvidersJson;

        public ICollection<Cipher> Ciphers { get; set; }
/*
        [IgnoreMap]
        public JsonDocument TwoFactorProvidersJson
        {
            get => _twoFactorProvidersJson;
            set
            {
                TwoFactorProviders = value?.ToString();
                _twoFactorProvidersJson = value;
            }
        }
*/        
    }

    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<DomainModels.User, User>().ReverseMap();
            CreateMap<User, DomainModels.Data.OrganizationUserUserDetails>()
            .ForMember(d=>d.UserId, opt=>opt.MapFrom(s=>s.Id))
            ;
        }
    }
}
