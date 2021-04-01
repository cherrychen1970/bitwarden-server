using System;
using System.Linq;
using System.Collections;


namespace Bit.Core.Models.EntityFramework
{
   public interface IEntityCreated 
    {
        //byte[] Timestamp { get; }
        DateTime CreationDate { get; }
        //DateTime? DeletedDate { get;  }
    }

   public interface IEntityUpdated
    {
        //byte[] Timestamp { get; }        
        DateTime RevisionDate { get;  }
        //DateTime? DeletedDate { get;  }
    }
}