using Bit.Core.Utilities;
using System;

namespace Bit.Core.Models
{
    public class Installation : IKey<Guid>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreationDate { get; internal set; } = DateTime.UtcNow;

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
        public void SetId(Guid id)=>Id=id;
    }
}
