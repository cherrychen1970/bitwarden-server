using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Bit.Core.Models;

namespace Bit.Core.Repositories.Noop
{
    public abstract class Repository<T, TId> : IRepository<T, TId>
        where TId : IEquatable<TId>
        where T : class, IKey<TId>
    {
        public Repository()
        {

        }

        protected string Schema { get; private set; } = "dbo";
        protected string Table { get; private set; } = typeof(T).Name;

        public virtual async Task<T> GetByIdAsync(TId id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task CreateAsync(T obj)
        {            
            throw new NotImplementedException();
        }

        public virtual async Task ReplaceAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public virtual async Task UpsertAsync(T obj)
        {
            if (obj.Id.Equals(default(TId)))
            {
                await CreateAsync(obj);
            }
            else
            {
                await ReplaceAsync(obj);
            }
        }

        public virtual async Task DeleteAsync(T obj)
        {
            throw new NotImplementedException();
        }
        public virtual async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
