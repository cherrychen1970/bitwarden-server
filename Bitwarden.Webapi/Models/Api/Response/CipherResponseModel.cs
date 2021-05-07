using System;
using Bit.Core.Models.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Bit.Core.Models.Api
{
    public class CipherResponseModel : ResponseModel
    {
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
            OrganizationId = cipher.OrganizationId?.ToString();            
            DeletedDate = cipher.DeletedDate;

            FolderId = cipher.FolderId?.ToString();
            Favorite = cipher.Favorite;
            Edit = cipher.Edit;
            ViewPassword = cipher.ViewPassword;
        }

        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public Enums.CipherType Type { get; set; }
        //public dynamic Data { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public CipherLoginModel Login { get; set; }
        public IEnumerable<CipherFieldModel> Fields { get; set; }
        public IEnumerable<CipherPasswordHistoryModel> PasswordHistory { get; set; }
        public IEnumerable<AttachmentResponseModel> Attachments { get; set; }
        public DateTime RevisionDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public string FolderId { get; set; }
        public bool Favorite { get; set; }
        public bool Edit { get; set; }
        public bool ViewPassword { get; set; }

        public CipherLoginData ToCipherLoginData(string data)
            => JsonConvert.DeserializeObject<CipherLoginData>(data);
    }

    public class CipherDetailsResponseModel : CipherResponseModel
    {
        public CipherDetailsResponseModel(Cipher cipher, string obj = "cipherDetails")
            : base(cipher,obj)
        {
            if (cipher.CollectionId.HasValue)
                CollectionIds = new Guid[] { cipher.CollectionId.Value };
        }

        public IEnumerable<Guid> CollectionIds { get; set; }
    }
}
