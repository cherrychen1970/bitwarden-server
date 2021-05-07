using System;

namespace Bit.Core.Models
{
    public class SsoConfig : IKey<long>
    {
        public long Id { get; set; }
        public bool Enabled { get; set; } = true;
        public Guid OrganizationId { get; set; }
        public string Data { get; set; }
        public DateTime CreationDate { get; internal set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; internal set; } = DateTime.UtcNow;
        
        public void SetNewId()
        {
            // nothing - int will be auto-populated
        }
    }
}
