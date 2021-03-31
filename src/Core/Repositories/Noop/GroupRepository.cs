using System;
using Bit.Core.Models.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories.Noop
{
    public class GroupRepository : Repository<Group, Guid>, IGroupRepository
    {
        public async Task<Tuple<Group, ICollection<SelectionReadOnly>>> GetByIdWithCollectionsAsync(Guid id)
        {
            return default(Tuple<Group, ICollection<SelectionReadOnly>>);
        }

        public async Task<ICollection<Group>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return new Group[] {};
        }

        public async Task<ICollection<Guid>> GetManyIdsByUserIdAsync(Guid organizationUserId)
        {
            return new Guid[] {};
        }

        public async Task<ICollection<Guid>> GetManyUserIdsByIdAsync(Guid id)
        {
            return new Guid[] {};
        }

        public async Task<ICollection<GroupUser>> GetManyGroupUsersByOrganizationIdAsync(Guid organizationId)
        {
            return new GroupUser[] {};
        }

        public async Task CreateAsync(Group obj, IEnumerable<SelectionReadOnly> collections)
        {
            obj.SetNewId();
            var objWithCollections = JsonConvert.DeserializeObject<GroupWithCollections>(JsonConvert.SerializeObject(obj));
            objWithCollections.Collections = collections.ToArrayTVP();
            throw new NotImplementedException();
        }

        public async Task ReplaceAsync(Group obj, IEnumerable<SelectionReadOnly> collections)
        {
            var objWithCollections = JsonConvert.DeserializeObject<GroupWithCollections>(JsonConvert.SerializeObject(obj));
            objWithCollections.Collections = collections.ToArrayTVP();

            throw new NotImplementedException();
        }

        public async Task DeleteUserAsync(Guid groupId, Guid organizationUserId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUsersAsync(Guid groupId, IEnumerable<Guid> organizationUserIds)
        {
            throw new NotImplementedException();
        }

        public class GroupWithCollections : Group
        {
            public DataTable Collections { get; set; }
        }
    }
}
