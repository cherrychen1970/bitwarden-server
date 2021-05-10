using System;
using Bit.Core.Models.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Bit.Core.Models.Api
{
    public class CipherResponseModel : ResponseModel
    {
        public CipherResponseModel(UserCipher cipher, string obj = "cipher")
            : this((Cipher)cipher, obj)
        {
            FolderId = cipher.FolderId;
        }
        public CipherResponseModel(OrganizationCipher cipher, string obj = "cipher")
            : this((Cipher)cipher, obj)
        {
            OrganizationId = cipher.OrganizationId;
            Edit = cipher.Edit;
            ViewPassword = cipher.ViewPassword;
            if (cipher.CollectionId.HasValue)
                CollectionIds = new Guid[] { cipher.CollectionId.Value };
        }
        public CipherResponseModel(Cipher cipher, string obj = "cipher")
            : base(obj)
        {
            Id = cipher.Id.ToString();
            Type = cipher.Type;

            CipherData cipherData;
            if (cipher.Type == Enums.CipherType.Login)
            {
                cipherData = cipher.Data;
                Login = new CipherLoginModel((CipherLoginData)cipherData);
            }
            else
                throw new ArgumentException("Unsupported type: " + nameof(Type) + ".");

            Name = cipherData.Name;
            Notes = cipherData.Notes;
            Fields = cipherData.Fields?.Select(f => new CipherFieldModel(f));
            PasswordHistory = cipherData.PasswordHistory?.Select(ph => new CipherPasswordHistoryModel(ph));
            RevisionDate = cipher.RevisionDate;
            DeletedDate = cipher.DeletedDate;
            Favorite = cipher.Favorite;

        }

        public string Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Enums.CipherType Type { get; set; }
        //public dynamic Data { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public CipherLoginModel Login { get; set; }
        public IEnumerable<CipherFieldModel> Fields { get; set; }
        public IEnumerable<CipherPasswordHistoryModel> PasswordHistory { get; set; }
        public DateTime RevisionDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public Guid? FolderId { get; set; }
        public bool Favorite { get; set; }
        public bool Edit { get; set; }
        public bool ViewPassword { get; set; }
public IEnumerable<Guid> CollectionIds { get; set; }
        public CipherLoginData ToCipherLoginData(string data)
            => JsonConvert.DeserializeObject<CipherLoginData>(data);
    }

    public class CipherDetailsResponseModel : CipherResponseModel
    {
        public CipherDetailsResponseModel(UserCipher cipher, string obj = "cipherDetails")
            : base(cipher, obj)
        {}
        public CipherDetailsResponseModel(OrganizationCipher cipher, string obj = "cipherDetails")
            : base(cipher, obj)
        {}
    }
}
