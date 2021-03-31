using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{
    public class OrganizationUser : Table.OrganizationUser
    {
        virtual public Organization Organization {get;set;}
        virtual public User User {get;set;}
        //public ICollection<Cipher> Ciphers { get; set; }
        
    }

    public class OrganizationUserProfile : Profile
    {
        public OrganizationUserProfile()
        {
            CreateMap<Table.OrganizationUser, OrganizationUser>().ReverseMap();
            CreateMap<Table.OrganizationUser, Data.OrganizationUserUserDetails>();
            CreateMap<Table.OrganizationUser, Data.OrganizationUserOrganizationDetails>();
        }
    }
}
