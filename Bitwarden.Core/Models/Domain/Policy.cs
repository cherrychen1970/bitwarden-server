using System;
using Bit.Core.Enums;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class Policy : IKey<Guid>
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public PolicyType Type { get; set; }
        public string Data { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreationDate { get; internal set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; internal set; } = DateTime.UtcNow;

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
        public void SetId(Guid id)=>Id=id;
    }
}
