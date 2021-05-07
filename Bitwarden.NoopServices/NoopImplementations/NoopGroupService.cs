using System;
using System.Threading.Tasks;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.Core.Repositories;
using System.Collections.Generic;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    public class NoopGroupService : IGroupService
    {
        public NoopGroupService()
        {
        }

        public async Task SaveAsync(Group group, IEnumerable<SelectionReadOnly> collections = null)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Group group)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUserAsync(Group group, Guid organizationUserId)
        {
            throw new NotImplementedException();
        }
    }
}
