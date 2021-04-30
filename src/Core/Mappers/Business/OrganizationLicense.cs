using Bit.Core.Enums;
using Bit.Core.Models.Table;
using Bit.Core.Services;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AutoMapper;

using Bit.Core.Models.Data;
using Bit.Core.Utilities;

namespace Bit.Core.Mappers.Business
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Models.Business.OrganizationLicense, Models.Table.Organization>()
                .ForMember(d => d.ExpirationDate, opt => opt.MapFrom(s => s.Expires))
                .ReverseMap();

            CreateMap<Models.Table.OrganizationUser, OrganizationMembership>()
            .ForMember(x => x.Id, o => o.MapFrom(s => s.OrganizationId))
            .ForMember(x => x.Permissions, o => o.MapFrom(s => CoreHelpers.LoadClassFromJsonData<Permissions>(s.Permissions)))
            ;
        }
    }
}
