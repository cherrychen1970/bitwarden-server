using System;
using System.Threading.Tasks;
using Bit.Core.Models;
using System.Collections.Generic;

namespace Bit.Core.Repositories
{
    public interface ICollectionCipherRepository
    {
        Task<ICollection<CollectionCipher>> GetManyByUserIdAsync(Guid userId);
        Task<ICollection<CollectionCipher>> GetManyByOrganizationIdAsync(Guid organizationId);
        Task<ICollection<CollectionCipher>> GetManyByUserIdCipherIdAsync(Guid userId, Guid cipherId);        
    }
}
