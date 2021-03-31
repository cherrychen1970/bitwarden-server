using System.Collections.Generic;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Core.Models.Data;

namespace Bit.Core.Models.EntityFramework
{
    public class Cipher : Table.Cipher
    {
        public Cipher() {
            CollectionCiphers = new HashSet<CollectionCipher>();
        }
        private JsonDocument _dataJson;
        private JsonDocument _attachmentsJson;
        private JsonDocument _favoritesJson;
        private JsonDocument _foldersJson;

        public User User { get; set; }
        public Organization Organization { get; set; }

        public ICollection<CollectionCipher> CollectionCiphers {get;set;}

        /*
        [IgnoreMap]
        [NotMapped]
        public JsonDocument DataJson
        {
            get => _dataJson;
            set
            {
                Data = value?.ToString();
                _dataJson = value;
            }
        }
        [IgnoreMap]
        [NotMapped]
        public JsonDocument AttachmentsJson
        {
            get => _attachmentsJson;
            set
            {
                Attachments = value?.ToString();
                _attachmentsJson = value;
            }
        }
        [IgnoreMap]
        [NotMapped]
        public JsonDocument FavoritesJson
        {
            get => _favoritesJson;
            set
            {
                Favorites = value?.ToString();
                _favoritesJson = value;
            }
        }
        [IgnoreMap]
        [NotMapped]
        public JsonDocument FoldersJson
        {
            get => _foldersJson;
            set
            {
                Folders = value?.ToString();
                _foldersJson = value;
            }
        }
        */
    }

    public class CipherMapperProfile : Profile
    {
        public CipherMapperProfile()
        {
            CreateMap<Table.Cipher, Cipher>().ReverseMap();
            CreateMap<Cipher, CipherDetails>().ReverseMap();
            // temp
            CreateMap<Table.Grant, Table.Grant>();
            CreateMap<Table.Device, Table.Device>();
        }
    }
}
