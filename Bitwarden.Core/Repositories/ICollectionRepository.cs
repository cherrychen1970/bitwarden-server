using System;
using System.Threading.Tasks;
using Bit.Core.Models;
using System.Collections.Generic;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories
{
    public interface ICollectionRepository : IRepository<Collection, Guid>
    {
        Task<int> GetCountByOrganizationIdAsync(Guid organizationId);
        Task<Tuple<Collection, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id);
        Task<Tuple<Collection, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id, Guid userId);
        Task<ICollection<Collection>> GetManyByOrganizationIdAsync(Guid organizationId);
        Task<Collection> GetByIdAsync(Guid id, Guid userId);
        Task<ICollection<Collection>> GetManyByUserIdAsync(Guid userId);
        Task DeleteUserAsync(Guid collectionId, Guid organizationUserId);
        Task UpdateUsersAsync(Guid id, IEnumerable<SelectionReadOnly> users);
        Task<ICollection<SelectionReadOnly>> GetManyUsersByIdAsync(Guid id);
    }
}
