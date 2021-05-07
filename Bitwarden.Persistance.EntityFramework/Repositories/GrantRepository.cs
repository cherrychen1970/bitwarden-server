using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bit.Core.Models;
using Bit.Core.Repositories;


namespace Bit.Infrastructure.EntityFramework
{
    public class GrantRepository : BaseRepository<Grant>,  IGrantRepository
    {
        public GrantRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<Grant> GetByKeyAsync(string key)
        {
            return await GetOne<Grant>(x=>x.Key==key);
        }

        public async Task<ICollection<Grant>> GetManyAsync(string subjectId, string sessionId,
            string clientId, string type)
        {
            return await GetMany<Grant>(x=>x.SubjectId==subjectId && x.SessionId==sessionId && x.ClientId==clientId && x.Type==type);
        }

        public async Task SaveAsync(Grant obj)
        {
            var exist = dbSet.Find(obj.Key);
            if (exist==null)
            {
                dbContext.Add(obj);                
            }
            else
            {
                Mapper.Map(obj,exist);               
            }                        
            
            await SaveChangesAsync();       
        }

        public async Task DeleteByKeyAsync(string key)
        {
            var entity = await GetOne(x=>x.Key==key);
            await base.DeleteAsync(entity);
        }

        public async Task DeleteManyAsync(string subjectId, string sessionId, string clientId, string type)
        {
            var entities = await GetMany(x=>x.SubjectId==subjectId && x.SessionId==sessionId && x.ClientId==clientId && x.Type==type);
            await base.DeleteManyAsync(entities);           
        }
    }
}
