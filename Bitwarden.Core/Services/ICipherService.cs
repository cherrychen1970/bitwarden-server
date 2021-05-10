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
        Task SaveAsync(UserCipher ipher);
        Task DeleteAsync(UserCipher cipher);
        Task SaveAsync(OrganizationCipher cipher);
        Task DeleteAsync(OrganizationCipher cipher);
        Task DeleteManyAsync(IEnumerable<Guid> cipherIds);
        Task DeleteManyAsync(IEnumerable<Guid> cipherIds, Guid organizationId);
        Task PurgeAsync(Guid organizationId);
        Task MoveManyAsync(IEnumerable<Guid> cipherIds, Guid destinationFolderId);
        Task SaveFolderAsync(Folder folder);
        Task DeleteFolderAsync(Folder folder);
        Task SaveCollectionsAsync(OrganizationCipher cipher, Guid collectionId);
        Task ImportCiphersAsync(List<Folder> folders, List<UserCipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> folderRelationships);
        Task ImportCiphersAsync(List<Collection> collections, List<OrganizationCipher> ciphers,
            IEnumerable<KeyValuePair<int, int>> collectionRelationships);
        Task SoftDeleteAsync(UserCipher cipher);
        Task SoftDeleteAsync(OrganizationCipher cipher);
        Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds);
        Task SoftDeleteManyAsync(IEnumerable<Guid> cipherIds, Guid organizationId);
        Task RestoreAsync(UserCipher cipher);
        Task RestoreAsync(OrganizationCipher cipher);
        Task RestoreManyAsync(IEnumerable<UserCipher> ciphers);
    }
}
