using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class Folder : IKey<Guid>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get;  set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get;  set; } = DateTime.UtcNow;

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
    }
}
