using System;

namespace Bit.Core.Models
{
    public interface IReferenceable
    {
        Guid Id { get; }
        string ReferenceData { get; set; }
        Enums.ReferenceEventSource source {get;}        
    }
}
