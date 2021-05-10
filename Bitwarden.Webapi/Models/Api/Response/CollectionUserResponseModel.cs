using System;
using Bit.Core.Models.Data;

namespace Bit.Core.Models.Api
{
    public class CollectionUserResponseModel
    {
        public CollectionUserResponseModel(CollectionAssigned selection)
        {
            Id = selection.UserId;
            ReadOnly = selection.ReadOnly;
            HidePasswords = selection.HidePasswords;
        }

        public Guid Id { get; set; }
        public bool ReadOnly { get; set; }
        public bool HidePasswords { get; set; }
    }
}
