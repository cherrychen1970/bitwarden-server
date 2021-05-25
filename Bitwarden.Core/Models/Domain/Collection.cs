using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class CollectionAccessProfile
    {
        public Guid OrganizationId { get; set; }
        public Enums.CollectionAccessType ReadAccess {get;set;} = Enums.CollectionAccessType.All;
    }    
    public class Collection : BaseModel
    {        
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }        
        //public Enums.CollectionAccessType ReadAccess {get;set;} = Enums.CollectionAccessType.All;
        //public Enums.CollectionAccessType WriteAccess {get;set;} = Enums.CollectionAccessType.All;
        public bool AdminOnly { get; set; }=false;        
        public bool ReadOnly { get; set; }=false;        
        public void SetNewId() =>       
           Id = Guid.NewGuid();

    }
}
