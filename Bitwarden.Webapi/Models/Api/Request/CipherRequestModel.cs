using System;
using System.ComponentModel.DataAnnotations;
using Bit.Core.Utilities;
using Bit.Core.Models;
using Bit.Core.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models.Data;
using Newtonsoft.Json.Linq;

namespace Bit.Core.Models.Api
{
    public class CipherRequestModel
    {
        public CipherType Type { get; set; }
        //public Guid? OrganizationId { get; set; }
        public Guid? CollectionId {get;set;}
        public Guid? FolderId { get; set; }
        public bool Favorite { get; set; }
        [Required]
        [EncryptedString]
        [EncryptedStringLength(1000)]
        public string Name { get; set; }
        [EncryptedString]
        [EncryptedStringLength(10000)]
        public string Notes { get; set; }
        public IEnumerable<CipherFieldModel> Fields { get; set; }
        public IEnumerable<CipherPasswordHistoryModel> PasswordHistory { get; set; }
        public CipherLoginModel Login { get; set; }
        public DateTime? LastKnownRevisionDate { get; set; } = null;

        public OrganizationCipher ToOrganizationCipher(Guid orgId)
        {
            var cipher = new OrganizationCipher
            {
                Type = Type,
                OrganizationId = orgId,
                CollectionId = CollectionId,
                Edit = true,
                ViewPassword = true,
            };
            return (OrganizationCipher)ToCipher((Cipher)cipher);
        }
        public OrganizationCipher ToOrganizationCipher(OrganizationCipher cipher)
        {
            cipher.Edit = true;
            cipher.ViewPassword = true;
            return (OrganizationCipher)ToCipher((Cipher)cipher);
        }
        public UserCipher ToCipher(Guid userId)
        {
            var cipher = new UserCipher
            {
                UserId = userId,
                Type = Type,
            };
            return (UserCipher)ToCipher((Cipher)cipher);
        }
        public UserCipher ToCipher(UserCipher existingCipher)
        {
            existingCipher.FolderId = FolderId;
            return (UserCipher)ToCipher((Cipher)existingCipher);
        }
        public Cipher ToCipher(Cipher existingCipher)
        {
            existingCipher.Favorite = Favorite;

            switch (existingCipher.Type)
            {
                case CipherType.Login:
                    existingCipher.Data = this.Login.ToCipherLoginData(Name);
                    break;
                default:
                    throw new ArgumentException("Unsupported type: " + nameof(Type) + ".");
            }
            return existingCipher;
        }
    }

    public class CipherWithIdRequestModel : CipherRequestModel
    {
        [Required]
        public Guid? Id { get; set; }
    }

    public class CipherCollectionsRequestModel
    {
        [Required]
        public IEnumerable<Guid> CollectionIds { get; set; }
    }

    public class CipherBulkDeleteRequestModel
    {
        [Required]
        public IEnumerable<Guid> Ids { get; set; }
        public Guid? OrganizationId { get; set; }
    }

    public class CipherBulkRestoreRequestModel
    {
        [Required]
        public IEnumerable<Guid> Ids { get; set; }
    }

    public class CipherBulkMoveRequestModel
    {
        [Required]
        public IEnumerable<Guid> Ids { get; set; }
        public Guid FolderId { get; set; }
    }
}
