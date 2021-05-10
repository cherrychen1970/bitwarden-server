using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models;
using Bit.Core.Models.Data;
using Bit.Core.Enums;
using Entities = Bit.Core.Entities;
namespace Bit.Core.Repositories
{
    public interface IOrganizationUserRepository : IRepository<OrganizationMembershipProfile, Guid>
    {
        Task<int> GetCountAsync(Expression<Func<Entities.OrganizationUser, bool>> expression);
        //Task<int> GetCountByOrganizationIdAsync(Guid organizationId);
        //Task<int> GetCountByFreeOrganizationAdminUserAsync(Guid userId);
        //Task<int> GetCountByOnlyOwnerAsync(Guid userId);
        //Task<ICollection<OrganizationMembershipProfile>> GetManyAsync(Expression<Func<Entities.OrganizationUser, bool>> expression);
       // Task<ICollection<TResult>> GetManyAsync<TResult>(Expression<Func<Entities.OrganizationUser, bool>> expression);
        Task<bool> Any(Expression<Func<Entities.OrganizationUser, bool>> expression);
        Task<OrganizationMembershipProfile> GetOneAsync(OrganizationMembership membership);
        Task<ICollection<OrganizationMembershipProfile>> GetManyAsync(IEnumerable<OrganizationMembership> memberships);
        Task<ICollection<OrganizationMembershipProfile>> GetManyByUserAsync(Guid userId, OrganizationUserStatusType statusType = OrganizationUserStatusType.Confirmed);
        Task<ICollection<OrganizationMembership>> GetMemberships(Guid userId);
       // Task<OrganizationMembership> GetMembership(Guid id);
        Task<ICollection<OrganizationMembershipProfile>> GetManyByOrganizationAsync(Guid organizationId, OrganizationUserType? type=null);
        // Task<ICollection<OrganizationUser>> GetManyByManyUsersAsync(IEnumerable<Guid> userIds);
        //Task<OrganizationMembershipProfile> GetByOrganizationAsync(Guid organizationId, Guid userId);
        //Task<int> GetCountByOrganizationAsync(Guid organizationId, string email, bool onlyRegisteredUsers);

        //Task<Tuple<OrganizationUser, ICollection<SelectionReadOnly>>> GetByIdWithCollectionsAsync(Guid id);
       // Task<ICollection<OrganizationUserUserDetails>> GetManyDetailsByOrganizationAsync1(Guid organizationId);
        //Task<ICollection<OrganizationUserOrganizationDetails>> GetManyDetailsByUserAsync(Guid userId, OrganizationUserStatusType? status = null);
        //Task CreateAsync(OrganizationMembershipProfile obj);//, IEnumerable<Models.CollectionAssigned> collections);
        //Task ReplaceAsync(OrganizationMembershipProfile obj, IEnumerable<Models.CollectionAssigned> collections);
    }
}
