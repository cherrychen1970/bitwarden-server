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

        override public async Task CreateAsync(UserCipher cipher)
        {                    
            var entity = Mapper.Map<EFModel.Cipher>(cipher);            
            dbContext.Add(entity);
            await SaveChangesAsync();
            cipher.Id = entity.Id;            
            //await base.CreateAsync(cipher);                        
            //await dbContext.SaveChangesAsync();
        }


        public async Task ReplaceAsync(UserCipher obj)
        {
            await base.ReplaceAsync(obj);
        }

        public async Task UpsertAsync(UserCipher cipher)
        {
            if (cipher.Id.Equals(default))
            {
                await CreateAsync(cipher);
            }
            else
            {
                await ReplaceAsync(cipher);
            }
        }

        public async Task UpdatePartialAsync(UserCipher cipher, Guid? folderId, bool favorite)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            throw new NotImplementedException();
        }


        public async Task MoveAsync(IEnumerable<Guid> ids, Guid folderId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task PurgeAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateManyAsync(IEnumerable<UserCipher> ciphers,Guid userId)
        {
            if (!ciphers.Any())
            {
                return;
            }
            throw new NotImplementedException();
        }

        public async Task CreateAsync(IEnumerable<UserCipher> ciphers, IEnumerable<Folder> folders)
        {
            if (!ciphers.Any())
            {
                return;
            }


            throw new NotImplementedException();
        }

        public async Task SoftDeleteAsync(Guid id, Guid userId)
        {
            var cipher = dbSet.SingleOrDefault(x=>x.Id==id && x.UserId==userId);
            cipher.DeletedDate=null;
            await SaveChangesAsync();            
        }

        public async Task SoftDeleteManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            var ciphers = await GetMany(x=> ids.Contains(x.Id) && x.UserId==userId );
            ciphers.ToList().ForEach(x=>x.DeletedDate=null);
            await SaveChangesAsync();            
        }

        public async Task<DateTime> RestoreManyAsync(IEnumerable<Guid> ids, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

