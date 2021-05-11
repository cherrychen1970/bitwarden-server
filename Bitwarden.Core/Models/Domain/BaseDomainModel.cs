using System;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bit.Core.Models
{
    public class BaseModel : IKey<Guid>, IRevisable
    {
        public Guid Id { get; protected set;}
        public void SetId(Guid id)=>Id=id;
        public DateTime CreationDate { get; protected set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; protected set; } = DateTime.UtcNow;
    }
}