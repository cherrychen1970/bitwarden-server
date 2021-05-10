using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using Bit.Core.Repositories;
using Bit.Core.Models;
using Bit.Core.Models.Data;
using EFModel = Bit.Core.Entities;

namespace Bit.Infrastructure.EntityFramework
{
    public class CollectionRepository : Repository<Collection, EFModel.Collection, Guid>, ICollectionRepository
    {
        public CollectionRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public override async Task CreateAsync(Collection obj)
        {
            var entity = Mapper.Map<EFModel.Collection>(obj);
            dbContext.Add(entity);
            //return await Task.CompletedTask;
        }
        public async Task<ICollection<CollectionMember>> GetAssignments(OrganizationMembership membership)
        {
            return await dbContext.CollectionUsers
                .Where(x=>x.OrganizationUser.UserId==membership.UserId)
                .Where(x=>x.OrganizationUser.OrganizationId==membership.OrganizationId)
                .ProjectTo<CollectionMember>(MapperProvider).ToListAsync();
        }
        public async Task<ICollection<CollectionMember>> GetAssignments(Guid id)
        {
            return await dbContext.CollectionUsers
                .Where(x=>x.CollectionId==id)
                .ProjectTo<CollectionMember>(MapperProvider).ToListAsync();
        }

        public async Task<ICollection<Collection>> GetManyAsync(OrganizationMembership membership)
        {
            //return await GetMany<Collection>(x=>true);
            return await GetMany<Collection>(x => x.OrganizationId == membership.OrganizationId);
        }
        public async Task<ICollection<Collection>> GetManyAsync(IEnumerable<OrganizationMembership> memberships)
        {
            var orgIds = memberships.Select(x => x.OrganizationId).ToList();
            return await GetMany<Collection>(x => orgIds.Contains(x.OrganizationId));
        }
        public async Task<Collection> GetByIdAsync(Guid id, Guid userId)
        {
            var cu = dbContext.CollectionUsers.SingleOrDefault(x => x.CollectionId == id && x.OrganizationUser.UserId == userId);

            var item = await GetByIdAsync<Collection>(id);
            item.HidePasswords = cu.HidePasswords;
            item.ReadOnly = cu.ReadOnly;
            return item;
        }

        public async Task<ICollection<Collection>> GetManyByUserIdAsync(Guid userId)
        {
            var cus = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId).ToList();
            var ids = cus.Select(x => x.CollectionId).ToList();
            //TODO : fill HidePasword, readonly option
            return await GetMany<Collection>(x => ids.Contains(x.Id));
        }

        public async Task CreateUserAsync(Guid collectionId, Guid organizationUserId)
        {
            var cu = new EFModel.CollectionUser() { CollectionId = collectionId, OrganizationUserId = organizationUserId };
            dbContext.CollectionUsers.Add(cu);
            await SaveChangesAsync();
        }

        public async Task DeleteMembersAsync(Guid collectionId, OrganizationMembership membership)
        {
            var item = dbContext.CollectionUsers
                .Where(x => x.CollectionId == collectionId)
                .Where(x=>x.OrganizationUser.OrganizationId==membership.OrganizationId)
                .Where(x=> x.OrganizationUser.UserId == membership.UserId).SingleOrDefault();
            dbContext.CollectionUsers.Remove(item);
            await SaveChangesAsync();
        }

        public async Task UpdateMembersAsync(IEnumerable<CollectionMember> users)
        {
            throw new NotImplementedException();
            /*
            var ids = users.Select(x=>x.Id);
            var collectionUsers = dbContext.CollectionUsers.Where(x => x.CollectionId == id).ToList();
            foreach (var user in users)
            {
                var collectionUser = collectionUsers.SingleOrDefault(x => x.OrganizationUserId == user.Id);
                if (collectionUser == null)
                {
                    collectionUser = new EFModel.CollectionUser() { CollectionId = id, OrganizationUserId = user.Id };
                    dbContext.CollectionUsers.Add(collectionUser);
                }
                collectionUser.ReadOnly = user.ReadOnly;
                collectionUser.HidePasswords = user.HidePasswords;
            }
            await dbContext.SaveChangesAsync();
            */
        }
    }
}
