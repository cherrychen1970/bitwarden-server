using System;

namespace Bit.Core.Models
{
    public interface IRevisable
    {
        DateTime CreationDate { get; }
        DateTime RevisionDate { get; }
    }
}
