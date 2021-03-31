using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TableModel = Bit.Core.Models.Table;
using EFModel = Bit.Core.Models.EntityFramework;
using Bit.Core.Models.Table;
using System.Data;
using Dapper;
using Core.Models.Data;
using Bit.Core.Utilities;
using Newtonsoft.Json;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories.EntityFramework
{
    public class CipherRepository : Repository<TableModel.Cipher, EFModel.Cipher, Guid>, ICipherRepository
    {
        public CipherRepository(IMapper mapper, DatabaseContext context)
            : base(context, mapper)
        { }


        public async Task<CipherDetails> GetByIdAsync(Guid id, Guid userId)
        {
            var cipher = await base.GetByIdAsync<CipherDetails>(id);
            if (cipher != null && cipher.UserId == userId) return cipher;
            return default(CipherDetails);
        }

        public async Task<CipherOrganizationDetails> GetOrganizationDetailsByIdAsync(Guid id)
        {
            return await base.GetByIdAsync<CipherOrganizationDetails>(id);
        }

        public async Task<bool> GetCanEditByIdAsync(Guid userId, Guid cipherId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<CipherDetails>> GetManyByUserIdAsync(Guid userId, bool withOrganizations = true)
        {
            //throw new NotImplementedException();
            return await GetMany<CipherDetails>(x => x.UserId == userId);
        }

        public async Task<ICollection<Cipher>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return await GetMany<Cipher>(x => x.OrganizationId == organizationId);            
        }

        public async Task CreateAsync(Cipher cipher, IEnumerable<Guid> collectionIds)
        {
            cipher.SetNewId();            
            var entity = Mapper.Map<EFModel.Cipher>(cipher);
            //await base.CreateAsync(cipher);
            foreach (var collectionId in collectionIds)
            {
                var collectionCipher = new EFModel.CollectionCipher() {CipherId=cipher.Id,CollectionId=collectionId};
                entity.CollectionCiphers.Add(collectionCipher);               
            }

            dbSet.Add(entity);
            await dbContext.SaveChangesAsync();
            cipher.Id=entity.Id;
        }

        public async Task CreateAsync(CipherDetails cipher)
        {
            cipher.SetNewId();                        
            var entity = Mapper.Map<EFModel.Cipher>(cipher);            
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();
            cipher.Id = entity.Id;            
            //await base.CreateAsync(cipher);                        
            //await dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(CipherDetails cipher, IEnumerable<Guid> collectionIds)
        {
            cipher.SetNewId();
            var objWithCollections = JsonConvert.DeserializeObject<CipherDetailsWithCollections>(
                JsonConvert.SerializeObject(cipher));
            objWithCollections.CollectionIds = collectionIds.ToGuidIdArrayTVP();
            throw new NotImplementedException();
        }

        public async Task ReplaceAsync(CipherDetails obj)
        {
            throw new NotImplementedException();
        }

        public async Task UpsertAsync(CipherDetails cipher)
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

        public async Task<bool> ReplaceAsync(Cipher obj, IEnumerable<Guid> collectionIds)
        {
            var objWithCollections = JsonConvert.DeserializeObject<CipherWithCollections>(
                JsonConvert.SerializeObject(obj));
            objWithCollections.CollectionIds = collectionIds.ToGuidIdArrayTVP();

            throw new NotImplementedException();
        }

        public async Task UpdatePartialAsync(Guid id, Guid userId, Guid? folderId, bool favorite)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAttachmentAsync(CipherAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAttachmentAsync(Guid cipherId, string attachmentId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(IEnumerable<Guid> ids, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByIdsOrganizationIdAsync(IEnumerable<Guid> ids, Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task SoftDeleteByIdsOrganizationIdAsync(IEnumerable<Guid> ids, Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task MoveAsync(IEnumerable<Guid> ids, Guid? folderId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByOrganizationIdAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserKeysAndCiphersAsync(User user, IEnumerable<Cipher> ciphers, IEnumerable<Folder> folders)
        {
            throw new NotImplementedException();


            return Task.FromResult(0);
        }

        public async Task UpdateCiphersAsync(Guid userId, IEnumerable<Cipher> ciphers)
        {
            if (!ciphers.Any())
            {
                return;
            }


            throw new NotImplementedException();
        }

        public async Task CreateAsync(IEnumerable<Cipher> ciphers, IEnumerable<Folder> folders)
        {
            if (!ciphers.Any())
            {
                return;
            }


            throw new NotImplementedException();
        }

        public async Task CreateAsync(IEnumerable<Cipher> ciphers, IEnumerable<Collection> collections,
            IEnumerable<CollectionCipher> collectionCiphers)
        {
            if (!ciphers.Any())
            {
                return;
            }



            throw new NotImplementedException();
        }

        public async Task SoftDeleteAsync(IEnumerable<Guid> ids, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<DateTime> RestoreAsync(IEnumerable<Guid> ids, Guid userId)
        {
            throw new NotImplementedException();
        }

        public class CipherDetailsWithCollections : CipherDetails
        {
            public DataTable CollectionIds { get; set; }
        }

        public class CipherWithCollections : Cipher
        {
            public DataTable CollectionIds { get; set; }
        }
    }
}
