using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using TableModel = Bit.Core.Models.Table;
using EFModel = Bit.Core.Models.EntityFramework;
using Bit.Core.Models.Data;
using System.Data;
using Dapper;
using Core.Models.Data;
using Bit.Core.Utilities;
using Bit.Core.Enums;
using Newtonsoft.Json;


namespace Bit.Core.Repositories.EntityFramework
{
    public class OrganizationUserRepository : Repository<TableModel.OrganizationUser, EFModel.OrganizationUser, Guid>, IOrganizationUserRepository
    {
        public OrganizationUserRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }
        public async Task<int> GetCountByOrganizationIdAsync(Guid organizationId)
        {
            return await base.dbSet.Where(x => x.OrganizationId == organizationId).CountAsync();
        }

        public async Task<int> GetCountByFreeOrganizationAdminUserAsync(Guid userId)
        {
            return await base.dbSet.Where(x => x.UserId == userId).CountAsync();
        }

        public async Task<int> GetCountByOnlyOwnerAsync(Guid userId)
        {
            throw new NotImplementedException();
            // TODO : this is not exact
            return await base.dbSet.Where(x => x.UserId == userId && x.Type == 0 && x.Status == OrganizationUserStatusType.Confirmed).CountAsync();
        }

        public async Task<int> GetCountByOrganizationAsync(Guid organizationId, string email, bool onlyRegisteredUsers)
        {
            throw new NotImplementedException();
            return await base.dbSet.Where(x => x.OrganizationId == organizationId && x.Email == email && x.Status == OrganizationUserStatusType.Confirmed).CountAsync();
        }

        public async Task<TableModel.OrganizationUser> GetByOrganizationAsync(Guid organizationId, Guid userId)
        {
            return await GetOne<TableModel.OrganizationUser>(x => x.UserId == userId && x.OrganizationId == organizationId);
        }

        public async Task<ICollection<TableModel.OrganizationUser>> GetManyByUserAsync(Guid userId)
        {
            return await GetMany<TableModel.OrganizationUser>(x => x.UserId == userId);
        }

        public async Task<ICollection<TableModel.OrganizationUser>> GetManyByOrganizationAsync(Guid organizationId,
            OrganizationUserType? type)
        {
            return await GetMany<TableModel.OrganizationUser>(x => x.OrganizationId == organizationId && x.Type == type);
        }

        public async Task<Tuple<TableModel.OrganizationUser, ICollection<SelectionReadOnly>>> GetByIdWithCollectionsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrganizationUserUserDetails> GetDetailsByIdAsync(Guid id)
        {
            return await GetUserDetail(x=>x.Id==id);
        }
        public async Task<Tuple<OrganizationUserUserDetails, ICollection<SelectionReadOnly>>> GetDetailsByIdWithCollectionsAsync(Guid id)
        {
            throw new NotImplementedException();

        }

        public async Task<ICollection<OrganizationUserUserDetails>> GetManyDetailsByOrganizationAsync(Guid organizationId)
        {
            return await GetManyUserDetails(x => x.OrganizationId == organizationId);
        }
        public async Task<ICollection<OrganizationUserOrganizationDetails>> GetManyDetailsByUserAsync(Guid userId,
            OrganizationUserStatusType? status = null)
            {                
                return await GetManyOrganizationDetails(x => x.UserId == userId && (x.Status == status || status == null));
            }

#region  "helper function"
        private async Task<OrganizationUserUserDetails> GetUserDetail(Expression<Func<EFModel.OrganizationUser,bool>> expression)            
        {   
            var query =  dbSet.Where(expression);        
            var item = query.ProjectTo<TableModel.OrganizationUser>(MapperProvider).Single();

            var detail = await query
                .Select(x => x.User).ProjectTo<OrganizationUserUserDetails>(MapperProvider)
                .SingleAsync();
            
            Mapper.Map(item,detail);            
            return detail;
        }            

        private async Task<ICollection<OrganizationUserUserDetails>> GetManyUserDetails(Expression<Func<EFModel.OrganizationUser,bool>> expression)            
        {   
            var query =  dbSet.Where(expression);        
            var lookup = query.ProjectTo<TableModel.OrganizationUser>(MapperProvider).ToDictionary(x => x.UserId, y => y);

            var list = await query
                .Select(x => x.User).ProjectTo<OrganizationUserUserDetails>(MapperProvider)
                .ToListAsync();

            foreach (var item in list)            
                Mapper.Map(lookup[item.UserId],item);
            
            return list;

            //return await GetMany<OrganizationUserOrganizationDetails>(x => x.UserId == userId && (x.Status == status || status == null));
        }

        private async Task<ICollection<OrganizationUserOrganizationDetails>> GetManyOrganizationDetails(Expression<Func<EFModel.OrganizationUser,bool>> expression)            
        {   
            var query =  dbSet.Where(expression);        
            var lookup = query.ProjectTo<TableModel.OrganizationUser>(MapperProvider).ToDictionary(x => x.OrganizationId, y => y);

            var list = await query
                .Select(x => x.Organization).ProjectTo<OrganizationUserOrganizationDetails>(MapperProvider)
                .ToListAsync();

            foreach (var item in list)            
                Mapper.Map(lookup[item.OrganizationId],item);
            
            return list;

            //return await GetMany<OrganizationUserOrganizationDetails>(x => x.UserId == userId && (x.Status == status || status == null));
        }

        private async Task<OrganizationUserOrganizationDetails> GetOrganizationDetail(Expression<Func<EFModel.OrganizationUser,bool>> expression)            
        {   
            var query =  dbSet.Where(expression);        
            var item = query.ProjectTo<TableModel.OrganizationUser>(MapperProvider).Single();

            var detail = await query
                .Select(x => x.Organization).ProjectTo<OrganizationUserOrganizationDetails>(MapperProvider)
                .SingleAsync();
            
            Mapper.Map(item,detail);            
            return detail;
        }
#endregion
        public async Task<OrganizationUserOrganizationDetails> GetDetailsByUserAsync(Guid userId,
            Guid organizationId, OrganizationUserStatusType? status = null)
        {
            return await GetOrganizationDetail(x => x.UserId == userId && x.OrganizationId == organizationId && (x.Status == status || status == null));
        }

        public async Task UpdateGroupsAsync(Guid orgUserId, IEnumerable<Guid> groupIds)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(TableModel.OrganizationUser obj, IEnumerable<SelectionReadOnly> collections)
        {
            obj.SetNewId();
            await base.CreateAsync(obj);
            throw new NotImplementedException();

            var objWithCollections = JsonConvert.DeserializeObject<OrganizationUserWithCollections>(
                JsonConvert.SerializeObject(obj));
            objWithCollections.Collections = collections.ToArrayTVP();
        }

        public async Task ReplaceAsync(TableModel.OrganizationUser obj, IEnumerable<SelectionReadOnly> collections)
        {
            throw new NotImplementedException();
            var objWithCollections = JsonConvert.DeserializeObject<OrganizationUserWithCollections>(
                JsonConvert.SerializeObject(obj));
            objWithCollections.Collections = collections.ToArrayTVP();
        }

        public class OrganizationUserWithCollections : TableModel.OrganizationUser
        {
            public DataTable Collections { get; set; }
        }

        public async Task<ICollection<TableModel.OrganizationUser>> GetManyByManyUsersAsync(IEnumerable<Guid> userIds)
        {
            return await GetMany<TableModel.OrganizationUser>(x => userIds.ToList().Contains(x.UserId.Value));
        }

        public async Task<TableModel.OrganizationUser> GetByOrganizationEmailAsync(Guid organizationId, string email)
        {
            return await GetOne<TableModel.OrganizationUser>(x => x.OrganizationId == organizationId && x.Email == email);
        }
    }
}
