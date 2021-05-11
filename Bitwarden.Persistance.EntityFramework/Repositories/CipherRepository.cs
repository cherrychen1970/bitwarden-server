using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AutoMapper;

using Bit.Core.Models;
using Bit.Core.Repositories;
using Bit.Core.Models.Data;
using Bit.Core.Utilities;
using EFModel = Bit.Core.Entities;


namespace Bit.Infrastructure.EntityFramework
{
    public class CipherRepository : Repository<UserCipher, EFModel.Cipher, Guid>, ICipherRepository
    {
        public CipherRepository(IMapper mapper, DatabaseContext context)
            : base(context, mapper)
        { }

        public async Task<UserCipher> GetByIdAsync(Guid id, Guid userId)
        {
            var cipher = await base.GetByIdAsync<UserCipher>(id);
            if (cipher != null && cipher.UserId == userId) return cipher;
            return null;
        }

        public async Task<ICollection<UserCipher>> GetManyAsync(Guid userId)
        {
            return await base.GetMany<UserCipher>(x => x.UserId == userId);
        }

        public async Task UpdatePartialAsync(Guid id, Guid? folderId, bool favorite)
        {
            var ciphers = await GetMany(x => x.Id == id);
            ciphers.ToList().ForEach(x =>
            {
                x.FolderId = folderId;
                x.Favorite = favorite;
            });
            await SaveChangesAsync();
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            var ciphers = await GetMany(x => ids.Contains(x.Id) && x.UserId == userId);
            dbSet.RemoveRange(ciphers);
            await SaveChangesAsync();
        }

        public async Task MoveAsync(IEnumerable<Guid> ids, Guid folderId, Guid userId)
        {
            var ciphers = await GetMany(x => ids.Contains(x.Id) && x.UserId == userId);
            ciphers.ToList().ForEach(x => x.FolderId = folderId);
            await SaveChangesAsync();
        }

        public async Task PurgeAsync(Guid userId)
        {
            var ciphers = await GetMany(x => x.UserId == userId);
            dbSet.RemoveRange(ciphers);
            await SaveChangesAsync();
        }

        public async Task UpdateManyAsync(IEnumerable<UserCipher> userCiphers, Guid userId)
        {
            var ids = userCiphers.Select(x=>x.Id).ToList();
            var lookup = userCiphers.ToDictionary(x=>x.Id,y=>y);
            var oldCiphers = await GetMany(x => ids.Contains(x.Id) && x.UserId == userId);
            if (!oldCiphers.Any())            
                return;
            foreach (var oldCipher in oldCiphers)
            {
                var from = lookup[oldCipher.Id];
                Mapper.Map(from,oldCipher);                
            }            
            await SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id, Guid userId)
        {
            var cipher = dbSet.SingleOrDefault(x => x.Id == id && x.UserId == userId);
            cipher.DeletedDate = DateTime.UtcNow;
            await SaveChangesAsync();
        }

        public async Task SoftDeleteManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            var ciphers = await GetMany(x => ids.Contains(x.Id) && x.UserId == userId);
            ciphers.ToList().ForEach(x => x.DeletedDate = DateTime.UtcNow);
            await SaveChangesAsync();
        }
        public async Task RestoreManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            var ciphers = await GetMany(x => ids.Contains(x.Id) && x.UserId == userId);
            ciphers.ToList().ForEach(x => x.DeletedDate = null);
            await SaveChangesAsync();
        }
    }
}

