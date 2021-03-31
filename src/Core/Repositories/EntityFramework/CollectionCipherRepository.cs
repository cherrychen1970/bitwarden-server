using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bit.Core.Models.Table;
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
    public class CollectionCipherRepository : ICollectionCipherRepository
    {
        protected IMapper Mapper { get; private set; }
        protected IConfigurationProvider MapperProvider => Mapper.ConfigurationProvider;
        protected DatabaseContext dbContext { get; set; }
        protected DbSet<EFModel.CollectionCipher> dbSet => dbContext.Set<EFModel.CollectionCipher>();
        public CollectionCipherRepository(DatabaseContext context, IMapper mapper)
        {
            Mapper = mapper;
            dbContext = context;
        }

        public virtual async Task<ICollection<TResult>> GetMany<TResult>(Expression<Func<EFModel.CollectionCipher, bool>> expression)
        {
            return await dbSet.Where(expression).ProjectTo<TResult>(MapperProvider).ToArrayAsync();
        }

        public async Task<ICollection<CollectionCipher>> GetManyByUserIdAsync(Guid userId)
        {
            var collections = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId).Select(x => x.CollectionId);
            return await GetMany<CollectionCipher>(x => collections.Contains(x.CollectionId));
        }

        public async Task<ICollection<CollectionCipher>> GetManyByOrganizationIdAsync(Guid organizationId)
        {
            return await GetMany<CollectionCipher>(x => x.Collection.OrganizationId == organizationId);
        }

        public async Task<ICollection<CollectionCipher>> GetManyByUserIdCipherIdAsync(Guid userId, Guid cipherId)
        {
            var collections = dbContext.CollectionUsers.Where(x => x.OrganizationUser.UserId == userId).Select(x => x.CollectionId);
            return await GetMany<CollectionCipher>(x => x.CipherId == cipherId && collections.Contains(x.CollectionId));
        }

        public async Task UpdateCollectionsAsync(Guid cipherId, Guid userId, IEnumerable<Guid> collectionIds)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCollectionsForAdminAsync(Guid cipherId, Guid organizationId, IEnumerable<Guid> collectionIds)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCollectionsForCiphersAsync(IEnumerable<Guid> cipherIds, Guid userId,
            Guid organizationId, IEnumerable<Guid> collectionIds)
        {
            throw new NotImplementedException();
        }
    }
}
