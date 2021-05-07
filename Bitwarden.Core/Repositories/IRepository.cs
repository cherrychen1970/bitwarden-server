using Bit.Core.Models;
using System;
using System.Threading.Tasks;

namespace Bit.Core.Repositories
{
    public interface IRepository<T, TId> where TId : IEquatable<TId> where T : class, IKey<TId>
    {
        Task<T> GetByIdAsync(TId id);
        Task CreateAsync(T obj);
        Task ReplaceAsync(T obj);
        Task UpsertAsync(T obj);
        Task DeleteAsync(T obj);
        Task<int> SaveChangesAsync();
    }
}
