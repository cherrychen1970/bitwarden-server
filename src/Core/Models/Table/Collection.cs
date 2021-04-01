using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models.Table
{
    public class Collection : ITableObject<Guid>
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; private set; } = DateTime.UtcNow;

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
    }
}
