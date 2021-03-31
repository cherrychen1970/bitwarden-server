using System;
using Bit.Core.Models.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Linq;
using Bit.Core.Enums;

namespace Bit.Core.Repositories.Noop
{
    public class PolicyRepository : Repository<Policy,Guid>, IPolicyRepository
    {
        public async Task<Policy> GetByOrganizationIdTypeAsync(Guid organizationId, PolicyType type)
        {
            return default(Policy);
        }

        public async Task<ICollection<Policy>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return new Policy[] {};
        }

        public async Task<ICollection<Policy>> GetManyByUserIdAsync(Guid userId)
        {
            return new Policy[] {};
        }
    }
}
