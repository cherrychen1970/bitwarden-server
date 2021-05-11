using Bit.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bit.Core.Repositories
{
    public interface IRepository<TModel, TKey> where TKey : IEquatable<TKey> where TModel : class, IKey<TKey>
    {
        Task<TModel> GetByIdAsync(TKey id);
        Task CreateAsync(TModel obj);
        Task ReplaceAsync(TModel obj);
        //Task UpsertAsync(TModel obj);
        Task DeleteAsync(TModel obj);
        Task<int> SaveChangesAsync();
    }
}
