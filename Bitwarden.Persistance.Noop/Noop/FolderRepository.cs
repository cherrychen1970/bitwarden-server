using System;
using Bit.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace Bit.Core.Repositories.Noop
{
    public class FolderRepository : Repository<Folder, Guid>, IFolderRepository
    {
        public async Task<Folder> GetByIdAsync(Guid id, Guid userId)
        {
            var folder = await GetByIdAsync(id);
            if (folder == null || folder.UserId != userId)
            {
                return null;
            }

            return folder;
        }

        public async Task<ICollection<Folder>> GetManyByUserIdAsync(Guid userId)
        {
            return new Folder[] {};
        }
    }
}
