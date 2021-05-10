using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
//using DomainModel = Bit.Core.Models;
using EFModel = Bit.Core.Entities;
using Bit.Core.Models;
using System.Data;

using Bit.Core.Repositories;
using Bit.Core.Utilities;
using Bit.Core.Enums;
using Newtonsoft.Json;


namespace Bit.Infrastructure.EntityFramework
{
    public class OrganizationUserRepository : Repository<OrganizationMembershipProfile, EFModel.OrganizationUser, Guid>, IOrganizationUserRepository
    {
        public OrganizationUserRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<ICollection<OrganizationMembership>> GetMemberships(Guid userId)
        => await GetMany<OrganizationMembership>(x => x.UserId == userId && x.Status == OrganizationUserStatusType.Confirmed);

        public async Task<OrganizationMembershipProfile> GetOneAsync(OrganizationMembership membership)
        {
            return await GetOne<OrganizationMembershipProfile>(x => x.UserId == membership.UserId && x.OrganizationId == membership.OrganizationId);
        }

        public async Task<ICollection<OrganizationMembershipProfile>> GetManyAsync(IEnumerable<OrganizationMembership> memberships)
        {
            var userIds = memberships.Select(x => x.UserId).ToList();
            var orgIds = memberships.Select(x => x.OrganizationId).ToList();
            return await GetMany<OrganizationMembershipProfile>(x => userIds.Contains(x.UserId) && orgIds.Contains(x.OrganizationId));
        }
        public async Task<ICollection<OrganizationMembershipProfile>> GetManyByUserAsync(Guid userId, OrganizationUserStatusType statusType = OrganizationUserStatusType.Confirmed)
        => await GetMany<OrganizationMembershipProfile>(x => x.UserId == userId && x.Status == statusType);

        public async Task<ICollection<OrganizationMembershipProfile>> GetManyByOrganizationAsync(Guid organizationId, OrganizationUserType? type)
        {
            return await GetMany<OrganizationMembershipProfile>(x => x.OrganizationId == organizationId && (type==null||x.Type == type));
        }

        public async Task CreateAsync(OrganizationMembershipProfile obj)//, IEnumerable<CollectionAssigned> collections)
        {
            await base.CreateAsync(obj);
            //throw new NotImplementedException();
        }
    }
}
