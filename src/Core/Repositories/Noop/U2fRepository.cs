using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Bit.Core.Models.Table;
using System.Data;
using Dapper;

namespace Bit.Core.Repositories.Noop
{
    public class U2fRepository : Repository<U2f, int>, IU2fRepository
    {


        public async Task<ICollection<U2f>> GetManyByUserIdAsync(Guid userId)
        {
            return new U2f[] {};
        }

        public async Task DeleteManyByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public override Task<U2f> GetByIdAsync(int id)
        {
            return  Task.FromResult(default(U2f));
        }

        public override Task ReplaceAsync(U2f obj)
        {
            throw new NotSupportedException();
        }

        public override Task UpsertAsync(U2f obj)
        {
            throw new NotSupportedException();
        }

        public override Task DeleteAsync(U2f obj)
        {
            throw new NotSupportedException();
        }
    }
}
