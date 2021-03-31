using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{
    public class Organization : Table.Organization
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

    public class OrganizationMapperProfile : Profile
    {
        public OrganizationMapperProfile()
        {
            CreateMap<Table.Organization, Organization>().ReverseMap();
            CreateMap<Organization, Data.OrganizationUserOrganizationDetails>()
            .ForMember(d=>d.OrganizationId, opt=>opt.MapFrom(s=>s.Id))
            ;
        }
    }
}
