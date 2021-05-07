using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories
{

    public interface ICipherRepository : IRepository<Cipher, Guid>
    {        
        Task<Cipher> GetByIdAsync(Guid id, Guid userId);                
        Task<ICollection<Cipher>> GetManyAsync(Guid userId);
        //Task<ICollection<Cipher>> GetManyByOrganizationIdAsync(Guid organizationId);
        //Task CreateAsync(Cipher cipher, IEnumerable<Guid> collectionIds);        
        Task CreateAsync(IEnumerable<Cipher> ciphers, IEnumerable<Folder> folders);
        //Task CreateAsync(IEnumerable<Cipher> ciphers, IEnumerable<Collection> collections, IEnumerable<CollectionCipher> collectionCiphers);
        Task UpdatePartialAsync(Cipher cipher, Guid? folderId, bool favorite);                
        Task MoveAsync(IEnumerable<Guid> ids, Guid? folderId, Guid userId);
        Task UpdateManyAsync(IEnumerable<Cipher> ciphers, Guid userId);
        //Task UpdateCollectionsAsync(Cipher cipher, IEnumerable<Guid> collectionIds);
        Task DeleteManyAsync(IEnumerable<Guid> ids, Guid userId);
        //Task DeleteByIdsOrganizationIdAsync(IEnumerable<Guid> ids, Guid organizationId);
        Task PurgeAsync(Guid userId);
        //Task DeleteByOrganizationIdAsync(Guid organizationId);        
        Task SoftDeleteManyAsync(IEnumerable<Guid> ids, Guid userId);
        //Task SoftDeleteByIdsOrganizationIdAsync(IEnumerable<Guid> ids, Guid organizationId);
        Task<DateTime> RestoreManyAsync(IEnumerable<Guid> ids, Guid userId);
    }
}
