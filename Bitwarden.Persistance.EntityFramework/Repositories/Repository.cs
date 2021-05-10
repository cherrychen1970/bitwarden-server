using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bit.Core.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Bit.Core.Repositories;
using Bit.Core.Entities;

namespace Bit.Infrastructure.EntityFramework
{
    public abstract class Repository<T, TEntity, TId> : BaseRepository<TEntity>,IRepository<T, TId>
        where TId : IEquatable<TId>
        where T : class, IKey<TId>
        where TEntity : class, IKey<TId>
    {

        public Repository(DatabaseContext context,IMapper mapper) : base(context,mapper)
        { }
        public virtual async Task<T> GetByIdAsync(TId id) => await GetByIdAsync<T>(id);                   
        public virtual async Task<TResult> GetByIdAsync<TResult>(TId id)
        {
            return await dbSet.Where(x => x.Id.Equals(id)).ProjectTo<TResult>(MapperProvider).SingleOrDefaultAsync();
        }        
        public virtual async Task CreateAsync(T obj)
        {
            var entity = Mapper.Map<TEntity>(obj);
            if (!obj.Id.Equals(default(TId)))
                entity.Id=obj.Id;
            dbContext.Add(entity);

            //TODO : this will be removed
            await SaveChangesAsync();  
            obj.Id = entity.Id;       
        }

        public virtual async Task ReplaceAsync(T obj)
        {
            var entity = await dbSet.FindAsync(obj.Id);
            if (entity != null)
            {
                Mapper.Map(obj,entity);
                //var mappedEntity = Mapper.Map<TEntity>(obj);
                //dbContext.Entry(entity).CurrentValues.SetValues(mappedEntity);
                await SaveChangesAsync();
            }
        }

        public virtual async Task UpsertAsync(T obj)
        {
            if (obj.Id.Equals(default(T)))
            {
                await CreateAsync(obj);
            }
            else
            {
                await ReplaceAsync(obj);
            }
        }     
        public virtual async Task DeleteAsync(TId id)
        {
            var entity = dbSet.Find(id);            
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T obj)
        {
            var entity = Mapper.Map<TEntity>(obj);
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }                     
    }

}
