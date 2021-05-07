using System;
using System.ComponentModel.DataAnnotations;
using Bit.Core.Utilities;
using Bit.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models.Data;

namespace Bit.Core.Models.Api
{
    public class CipherLoginModel
    {
        public CipherLoginModel() { }

        public CipherLoginModel(CipherLoginData data)
        {
            Uris = data.Uris?.Select(u => new CipherLoginUriModel(u))?.ToList();
            if (!Uris?.Any() ?? true)
            {
                Uri = data.Uri;
            }

            Username = data.Username;
            Password = data.Password;
            PasswordRevisionDate = data.PasswordRevisionDate;
            Totp = data.Totp;
        }

        public CipherLoginData ToCipherLoginData(string name)
        {
            var loginData = new CipherLoginData();
            loginData.Uris = this.Uris?.Where(u => u != null).Select(u => new CipherLoginData.CipherLoginUriData()
            {
                Uri = u.Uri,
                Match = u.Match
            });
            loginData.Name = name;
            loginData.Username = Username;
            loginData.Password = Password;
            loginData.PasswordRevisionDate = PasswordRevisionDate;
            loginData.Totp = Totp;
            return loginData;
        }

        [EncryptedString]
        [EncryptedStringLength(10000)]
        public string Uri
        {
            get => Uris?.FirstOrDefault()?.Uri;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (Uris == null)
                {
                    Uris = new List<CipherLoginUriModel>();
                }

                Uris.Add(new CipherLoginUriModel(value));
            }
        }
        public List<CipherLoginUriModel> Uris { get; set; }
        [EncryptedString]
        [EncryptedStringLength(1000)]
        public string Username { get; set; }
        [EncryptedString]
        [EncryptedStringLength(1000)]
        public string Password { get; set; }
        public DateTime? PasswordRevisionDate { get; set; }
        [EncryptedString]
        [EncryptedStringLength(1000)]
        public string Totp { get; set; }

        public class CipherLoginUriModel
        {
            public CipherLoginUriModel() { }

            public CipherLoginUriModel(string uri)
            {
                Uri = uri;
            }

            public CipherLoginUriModel(CipherLoginData.CipherLoginUriData uri)
            {
                Uri = uri.Uri;
                Match = uri.Match;
            }

            [EncryptedString]
            [EncryptedStringLength(10000)]
            public string Uri { get; set; }
            public UriMatchType? Match { get; set; } = null;
        }
    }
}
