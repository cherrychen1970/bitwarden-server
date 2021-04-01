using System;
using System.Linq;
using Bit.Core.Models.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq;
using Newtonsoft.Json;
using TableModel = Bit.Core.Models.Table;
using EFModel = Bit.Core.Models.EntityFramework;
using DataModel = Bit.Core.Models.Data;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories.EntityFramework
{
    public class CollectionRepository : Repository<TableModel.Collection, EFModel.Collection, Guid>, ICollectionRepository
    {
        public CollectionRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<int> GetCountByOrganizationIdAsync(Guid organizationId)
        {
            return await GetCountAsync(x => x.OrganizationId == organizationId);
        }

        public async Task<Tuple<Collection, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id)
        {
            var item = await GetOne<Collection>(x=>x.Id==id);
            return new Tuple<Collection, ICollection<SelectionReadOnly>>(item,new SelectionReadOnly[]{});            
        }

        public async Task<Tuple<CollectionDetails, ICollection<SelectionReadOnly>>> GetByIdWithGroupsAsync(Guid id, Guid userId)
        {
            var query = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId && x.CollectionId==id);
            if (!query.Any()) return null;

            var detail = await query.SingleAsync();
            var collection = query.Select(x=>x.Collection).ProjectTo<CollectionDetails>(MapperProvider).Single();
            collection.SetDetails(detail);
            return new Tuple<CollectionDetails, ICollection<SelectionReadOnly>>(collection,new SelectionReadOnly[]{});
        }

        public async Task<ICollection<Collection>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return await GetMany<Collection>(x => x.OrganizationId == organizationId);
        }

        public async Task<CollectionDetails> GetByIdAsync(Guid id, Guid userId)
        {
            var cu = dbContext.CollectionUsers.SingleOrDefault(x => x.CollectionId == id && x.OrganizationUser.UserId == userId);

            var item = await GetByIdAsync<CollectionDetails>(id);
            item.HidePasswords = cu.HidePasswords;
            item.ReadOnly = cu.ReadOnly;

            return item;
        }

        public async Task<ICollection<CollectionDetails>> GetManyByUserIdAsync(Guid userId)
        {
            var cus = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId).ToList();
            var ids = cus.Select(x => x.CollectionId).ToList();
            //TODO : fill HidePasword, readonly option
            return await GetMany<CollectionDetails>(x => ids.Contains(x.Id));
        }

        public async Task CreateAsync(Collection obj, IEnumerable<SelectionReadOnly> groups)
        {
            await base.CreateAsync(obj);            
        }

        public async Task ReplaceAsync(Collection obj, IEnumerable<SelectionReadOnly> groups)
        {
            var collection = dbSet.Find(obj.Id);
            Mapper.Map(obj,collection);
            await SaveChangesAsync();
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

        public class CollectionWithGroups : Collection
        {
            public DataTable Groups { get; set; }
        }
    }
}
