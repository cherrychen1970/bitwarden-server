using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using DomainModels = Bit.Core.Models;

namespace Bit.Core.Entities
{
    public class User : BaseGuidEntity, IEntityAuditable
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string MasterPassword { get; set; }
        public string MasterPasswordHint { get; set; }
        public string Culture { get; set; } = "en-US";
        public string SecurityStamp { get; set; }        
        public DateTime AccountRevisionDate { get; set; } = DateTime.UtcNow;
        public string Key { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string ApiKey { get; set; }        
        public int KdfIterations { get; set; } = 5000;
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public ICollection<Cipher> Ciphers { get; set; }
    }

    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, DomainModels.User>()
                .ReverseMap()
                .Ignore(x => x.Id);
            ;
        }
    }
}
