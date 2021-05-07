using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class Group : IKey<Guid>, IExternal
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public bool AccessAll { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get;  set; } = DateTime.UtcNow;

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
    }
}
