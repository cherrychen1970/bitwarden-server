﻿using System;
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

        public CollectionMember ToCollectionMember(Guid collectionId)
        {
            return new CollectionMember
            {
                CollectionId = collectionId,
                OrganizationUserId = Id,
                ReadOnly = ReadOnly,
                HidePasswords = HidePasswords,
            };
        }
        /*
        public CollectionMember ToCollectionAssigned(OrganizationMembershipProfile user)
        {
            return new CollectionMember
            {
                UserId = user.UserId,
                CollectionId = Id,
                ReadOnly = ReadOnly,
                HidePasswords = HidePasswords,
            };
        }
        */
    }
}
