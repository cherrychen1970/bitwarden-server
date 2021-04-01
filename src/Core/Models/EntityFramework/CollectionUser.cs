using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{
    public class CollectionUser : Table.CollectionUser
    {        
        public virtual Collection Collection { get; set; }
        public virtual OrganizationUser OrganizationUser { get; set; }
    }

    public class CollectionUserProfile : Profile
    {
        public CollectionUserProfile()
        {
            CreateMap<CollectionUser,Data.SelectionReadOnly>()                        
            .ForMember(dst=>dst.Id, opt=>opt.MapFrom(src=>src.OrganizationUserId));
        }
    }
}
