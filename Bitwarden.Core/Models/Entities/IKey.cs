using System;

namespace Bit.Core.Models
{
    public interface IEntityDates
    {
        DateTime CreationDate { get; internal set; } 
        DateTime RevisionDate { get; internal set; } 
    }

    public interface IKey<T> where T : IEquatable<T>
    {
        T Id { get; set; }
        //void SetNewId();
    }
}
