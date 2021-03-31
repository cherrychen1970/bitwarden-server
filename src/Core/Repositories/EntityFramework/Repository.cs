using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bit.Core.Models.Table;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bit.Core.Repositories.EntityFramework
{
    public abstract class BaseRepository<TEntity> 
        where TEntity : class
    {
        protected IMapper Mapper { get; private set; }
        protected IConfigurationProvider MapperProvider => Mapper.ConfigurationProvider;
        protected DatabaseContext dbContext { get; set; }
        protected DbSet<TEntity> dbSet => dbContext.Set<TEntity>();

        public BaseRepository(DatabaseContext context,IMapper mapper)
        {
            Mapper = mapper;
            dbContext = context;
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.Where(expression).CountAsync();                    
        }

        public virtual async Task<TEntity> GetOne(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.Where(expression).SingleOrDefaultAsync();
        }        

        public virtual async Task<ICollection<TEntity>> GetMany(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        } 
        public virtual async Task<TResult> GetOne<TResult>(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.Where(expression).ProjectTo<TResult>(MapperProvider).SingleOrDefaultAsync();
        }        

        public virtual async Task<ICollection<TResult>> GetMany<TResult>(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.Where(expression).ProjectTo<TResult>(MapperProvider).ToListAsync();
        }        

        public virtual async Task DeleteAsync(TEntity entity)
        {            
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }   
        public virtual async Task DeleteManyAsync(ICollection<TEntity>  entities)
        {            
            dbSet.RemoveRange(entities);
            await dbContext.SaveChangesAsync();
        }                 
    }

    public abstract class Repository<T, TEntity, TId> : BaseRepository<TEntity>,IRepository<T, TId>
        where TId : IEquatable<TId>
        where T : class, ITableObject<TId>
        where TEntity : class, ITableObject<TId>
    {

        public Repository(DatabaseContext context,IMapper mapper) : base(context,mapper)
        {
        }
        public virtual async Task<T> GetByIdAsync(TId id)
        {
            var entity = await dbSet.FindAsync(id);
            return entity as T;
        }        
        public virtual async Task<TResult> GetByIdAsync<TResult>(TId id)
        {
            return await Task.FromResult(dbSet.Where(x => x.Id.Equals(id)).ProjectTo<TResult>(MapperProvider).SingleOrDefault());
        }        
        public virtual async Task CreateAsync(T obj)
        {
            var entity = Mapper.Map<TEntity>(obj);
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();  
            obj.Id = entity.Id;       
        }

        public virtual async Task ReplaceAsync(T obj)
        {
            var entity = await dbSet.FindAsync(obj.Id);
            if (entity != null)
            {
                var mappedEntity = Mapper.Map<TEntity>(obj);
                dbContext.Entry(entity).CurrentValues.SetValues(mappedEntity);
                await dbContext.SaveChangesAsync();
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
