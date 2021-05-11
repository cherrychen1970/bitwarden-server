using System;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class Folder : BaseModel
    {       
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public void SetNewId()
        {
            Id = Guid.NewGuid();
        }
    }
}
