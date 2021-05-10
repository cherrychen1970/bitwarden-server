using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class Collection : IKey<Guid>
    {
        public Guid Id { get; set; }//=Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;
        public bool ReadOnly { get; set; }=false;
        public bool HidePasswords { get; set; } =false; 
        public void SetNewId()=>Id=Guid.NewGuid();      
    }
}
