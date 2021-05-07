using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models;
using Bit.Core.Models.Data;
using System;
using System.IO;

namespace Bit.Core.Services
{
    public interface ICipherService
    {
        Task SaveAsync(Cipher cipher);
        Task DeleteAsync(Cipher cipher);
        Task DeleteManyAsync(IEnumerable<Guid> cipherIds, Guid? organizationId = null);
        Task PurgeAsync(Guid organizationId);
        Task MoveManyAsync(IEnumerable<Guid> cipherIds, Guid? destinationFolderId);
        Task SaveFolderAsync(Folder folder);
        Task DeleteFolderAsync(Folder folder);
        Task SaveCollectionsAsync(Cipher cipher, Guid collectionId);
        Task ImportCiphersAsync(List<Folder> folders, List<Cipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> folderRelationships);
        Task ImportCiphersAsync(List<Collection> collections, List<Cipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> collectionRelationships, Guid importingUserId);
        Task SoftDeleteAsync(Cipher cipher);
        Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds, Guid? organizationId = null);
        Task RestoreAsync(Cipher cipher);
        Task RestoreManyAsync(IEnumerable<Cipher> ciphers, Guid restoringUserId);
    }
}
