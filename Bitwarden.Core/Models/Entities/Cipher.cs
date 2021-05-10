using System;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bit.Core.Models
{
    public class UserCipher : Cipher
    {
        public Guid UserId { get; set; }
        public Guid? FolderId { get; set; }
    }

    public class OrganizationCipher : Cipher
    {

        public Guid OrganizationId { get; set; }
        public Guid? CollectionId { get; set; }
        public bool Edit { get; set; } = true;
        public bool ViewPassword { get; set; } = true;

    }
    abstract public class Cipher : IKey<Guid>, ICloneable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Enums.CipherType Type { get; set; }
        //public string Data { get; set; }
        public CipherData Data { get; set; }
        //public string Favorites { get; set; }
        //public string Folders { get; set; }
        public bool Favorite { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public DateTime? DeletedDate { get; set; }

        object ICloneable.Clone() => Clone();
        public Cipher Clone()
        {
            var clone = CoreHelpers.CloneObject(this);
            clone.CreationDate = CreationDate;
            clone.RevisionDate = RevisionDate;

            return clone;
        }
    }
}
