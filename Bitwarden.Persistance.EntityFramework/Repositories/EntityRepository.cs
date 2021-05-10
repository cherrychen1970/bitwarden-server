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
        public virtual async Task<bool> Any(Expression<Func<TEntity,bool>> expression)
        {
            return await dbSet.AnyAsync(expression);                
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

        public async Task<int> SaveChangesAsync()
        {
            ConcurrencyCheck();
            FillAudit();
            return await dbContext.SaveChangesAsync();
        }

        virtual protected void ConcurrencyCheck()
        {
            var ChangeTracker = dbContext.ChangeTracker;
            //ChangeTracker.DetectChanges();

            var modified = ChangeTracker.Entries<IEntityUpdated>().Where(x => x.State == EntityState.Modified);

            foreach (var entity in modified)
            {
                if (entity.State == EntityState.Modified)
                {
                    var original=entity.OriginalValues.GetValue<DateTime>(nameof(IEntityUpdated.RevisionDate));
                    if (entity.Entity.RevisionDate!=original) {                        
                        throw new DbUpdateConcurrencyException(entity.Entity.RevisionDate.ToString());                        
                    }                       
                }
            }
        }   
        virtual protected void FillAudit()
        {
            var date  = DateTime.UtcNow;
            var ChangeTracker = dbContext.ChangeTracker;
            var addeds = ChangeTracker.Entries<IEntityCreated>().Where(x => x.State == EntityState.Added);
            foreach (var entity in addeds)
            {                
                entity.Property(nameof(IEntityCreated.CreationDate)).CurrentValue = date;
            }
            var modifieds = ChangeTracker.Entries<IEntityUpdated>().Where(x => x.State == EntityState.Modified);

            foreach (var entity in modifieds)
            {                
                entity.Property(nameof(IEntityUpdated.RevisionDate)).CurrentValue = date;
            }
        }
    }
}
