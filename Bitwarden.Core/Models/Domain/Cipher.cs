using System;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bit.Core.Models
{
    public class CipherAccessProfile
    {
        public Guid OrganizationId { get; set; }
        public Guid? CollectionId { get; set; }
    }

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
    abstract public class Cipher : BaseModel, ICloneable
    {
        public Enums.CipherType Type { get; set; }
        public CipherData Data { get; set; }
        public bool Favorite { get; set; }
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
