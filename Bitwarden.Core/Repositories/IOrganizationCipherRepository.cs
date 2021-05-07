using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models;

namespace Bit.Core.Repositories
{
    public interface IOrganizationCipherRepository : IRepository<Cipher, Guid>
    {        
        //Task SetOrganizationFilter(Guid organizationId);
        Task<Cipher> GetByIdAsync(Guid id, OrganizationMembership membership);
        Task<Entities.Cipher> GetEntityAsync(Guid id, OrganizationMembership membership);
        Task<ICollection<Cipher>> GetManyAsync(OrganizationMembership membership);
        Task<ICollection<Cipher>> GetManyAsync(IEnumerable<OrganizationMembership> memberships);             
        Task CreateAsync(IEnumerable<Cipher> ciphers);        
        Task DeleteManyAsync(IEnumerable<Guid> ids,OrganizationMembership membership);
        Task PurgeAsync(OrganizationMembership membership);
        Task DeleteAsync(Guid id,OrganizationMembership membership);        
        Task SoftDeleteManyAsync(IEnumerable<Guid> ids,OrganizationMembership membership);        
    }
}
