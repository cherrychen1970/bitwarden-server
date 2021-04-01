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

namespace Bit.Core.Mappers.Business
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Models.Business.OrganizationLicense, Models.Table.Organization>()
                .ForMember(d=>d.ExpirationDate,opt=>opt.MapFrom(s=>s.Expires))
                .ReverseMap();
        }
    }    
}
