using System;
using System.Threading.Tasks;
using Bit.Core.Models;
using System.Collections.Generic;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories
{
    public interface ICollectionRepository : IRepository<Collection, Guid>
    {
        Task<ICollection<CollectionMember>> GetAssignments(OrganizationMembership membership);
        Task<ICollection<CollectionMember>> GetAssignments(Guid id);
        Task<Collection> GetByIdAsync(Guid id, Guid userId);
        Task<ICollection<Collection>> GetManyAsync(OrganizationMembership membership);
        Task<ICollection<Collection>> GetManyAsync(IEnumerable<OrganizationMembership> memberships);
        //Task DeleteUserAsync(CollectionAssigned assigned);
        Task DeleteMembersAsync(Guid id, Guid orgUserId);
        Task UpdateMembersAsync(IEnumerable<CollectionMember> users);
    }
}
