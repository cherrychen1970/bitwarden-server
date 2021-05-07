using Bit.Core.Models;
using System;
using System.Threading.Tasks;

namespace Bit.Core.Repositories
{
    public interface ISsoUserRepository : IRepository<SsoUser, long>
    {
        Task DeleteAsync(Guid userId, Guid? organizationId);
    }
}
