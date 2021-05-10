using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels=Bit.Core.Models;


namespace Bit.Core.Entities
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
            CreateMap<CollectionUser,DomainModels.CollectionAssigned>()                        
            .ForMember(dst=>dst.UserId, opt=>opt.MapFrom(src=>src.OrganizationUser.UserId))
            .ReverseMap()
            .ForMember(dst=>dst.OrganizationUser,opt=>opt.Ignore())
            ;
            ;
        }
    }
}
