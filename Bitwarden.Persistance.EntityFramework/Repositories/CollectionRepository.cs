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

        public async Task<int> GetCountByOrganizationIdAsync(Guid organizationId)
        {
            return await GetCountAsync(x => x.OrganizationId == organizationId);
        }

        public override async Task CreateAsync(Collection obj)
        {
            var entity = Mapper.Map<EFModel.Collection>(obj);
            dbContext.Add(entity);            
            //return await Task.CompletedTask;
        }        

        public async Task<Tuple<Collection, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id)
        {
            var item = await GetOne<Collection>(x=>x.Id==id);
            return new Tuple<Collection, ICollection<SelectionReadOnly>>(item,new SelectionReadOnly[]{});            
        }

        public async Task<Tuple<Collection, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id, Guid userId)
        {
            var query = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId && x.CollectionId==id);
            if (!query.Any()) return null;

            var permission = await query.ProjectTo<CollectionUser>(MapperProvider).SingleAsync();
            var collection = query.Select(x=>x.Collection).ProjectTo<Collection>(MapperProvider).Single();
            collection.HidePasswords = permission.HidePasswords;
            collection.ReadOnly = permission.ReadOnly;            
            return new Tuple<Collection, ICollection<SelectionReadOnly>>(collection,new SelectionReadOnly[]{});
        }

        public async Task<ICollection<Collection>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return await GetMany<Collection>(x => x.OrganizationId == organizationId);
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

        public async Task DeleteUserAsync(Guid collectionId, Guid organizationUserId)
        {
            var item = dbContext.CollectionUsers.Single(x => x.CollectionId == collectionId && x.OrganizationUserId == organizationUserId);
            dbContext.CollectionUsers.Remove(item);
            await SaveChangesAsync();
        }

        public async Task UpdateUsersAsync(Guid id, IEnumerable<SelectionReadOnly> users)
        {
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
        }

        public async Task<ICollection<SelectionReadOnly>> GetManyUsersByIdAsync(Guid id)
        {
            return await dbContext.CollectionUsers.Where(x => x.CollectionId == id)
                .ProjectTo<SelectionReadOnly>(MapperProvider).ToArrayAsync();
        }
    }
}
