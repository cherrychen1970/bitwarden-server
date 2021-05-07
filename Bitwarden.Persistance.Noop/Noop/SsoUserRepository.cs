using Bit.Core.Models;
using Dapper;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Bit.Core.Repositories.Noop
{
    public class SsoUserRepository : Repository<SsoUser, long>, ISsoUserRepository
    {

        public async Task DeleteAsync(Guid userId, Guid? organizationId)
        {
            throw new NotImplementedException();

        }
    }
}
