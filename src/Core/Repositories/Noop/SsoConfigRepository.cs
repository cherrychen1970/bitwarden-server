using System;
using Bit.Core.Models.Table;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Linq;
using System.Collections.Generic;

namespace Bit.Core.Repositories.Noop
{
    public class SsoConfigRepository : Repository<SsoConfig, long>, ISsoConfigRepository
    {

        public async Task<SsoConfig> GetByOrganizationIdAsync(Guid organizationId)
        {
            return default(SsoConfig);
        }

        public async Task<SsoConfig> GetByIdentifierAsync(string identifier)
        {
            return default(SsoConfig);
        }

        public async Task<ICollection<SsoConfig>> GetManyByRevisionNotBeforeDate(DateTime? notBefore)
        {
            return new SsoConfig[] {};
        }
    }
}
