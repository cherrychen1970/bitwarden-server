using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TableModel = Bit.Core.Models.Table;
using EFModel = Bit.Core.Models.EntityFramework;
using Bit.Core.Models.Table;
using System.Data;
using Dapper;
using Core.Models.Data;
using Bit.Core.Utilities;
using Newtonsoft.Json;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories.EntityFramework
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
            dbContext.Add(obj);
            await dbContext.SaveChangesAsync();       
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
