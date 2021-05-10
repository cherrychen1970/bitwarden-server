using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Bit.Core.Models;
using Bit.Core.Models.Data;

namespace Bit.Core.Models.Api
{
    public class CollectionUserRequestModel
    {
        [Required]
        public Guid Id { get; set; }
        public bool ReadOnly { get; set; }
        public bool HidePasswords { get; set; }

        public CollectionAssigned ToCollectionAssigned(Collection c)
        {
            return new CollectionAssigned
            {
                CollectionId = c.Id,
                UserId = Id,
                ReadOnly = ReadOnly,
                HidePasswords = HidePasswords,
            };
        }
        public CollectionAssigned ToCollectionAssigned(OrganizationMembership user)
        {
            return new CollectionAssigned
            {
                UserId = user.UserId,
                CollectionId = Id,
                ReadOnly = ReadOnly,
                HidePasswords = HidePasswords,
            };
        }
    }
}
