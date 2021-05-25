using System;
using Bit.Core.Models;
using System.Collections.Generic;
using Bit.Core.Models.Data;
using System.Linq;

namespace Bit.Core.Models.Api
{
    public class CollectionResponseModel : ResponseModel
    {
        public CollectionResponseModel(Collection collection, string obj = "collection")
            : base(obj)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Id = collection.Id.ToString();
            OrganizationId = collection.OrganizationId.ToString();
            Name = collection.Name;
            ExternalId = collection.ExternalId;
            AdminOnly = collection.AdminOnly;       
            ReadOnly = collection.ReadOnly;       
        }

        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public bool AdminOnly { get; set; }        
        public bool ReadOnly { get; set; }        
    }
}



