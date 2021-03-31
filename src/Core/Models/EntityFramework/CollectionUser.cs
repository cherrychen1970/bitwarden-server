using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;

namespace Bit.Core.Models.EntityFramework
{
    public class CollectionUser 
    {        
        virtual public int Id {get;set;}
        public Guid CollectionId {get;set;}
        public Guid OrganizationUserId {get;set;}

        public virtual bool ReadOnly {get;set;}
        public virtual bool HidePasswords {get;set;}
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
