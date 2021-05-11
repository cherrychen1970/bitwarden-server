using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using Bit.Core.Repositories;
using Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using Bit.Core.Models.Data;
using Bit.Core.Utilities;
using EFModel = Bit.Core.Entities;


namespace Bit.Infrastructure.EntityFramework
{
    public class OrganizationCipherRepository : Repository<OrganizationCipher, EFModel.Cipher, Guid>, IOrganizationCipherRepository
    {
        public OrganizationCipherRepository(IMapper mapper, DatabaseContext context)
            : base(context, mapper)
        { }

        public async Task<OrganizationCipher> GetByIdAsync(Guid id, OrganizationMembership membership)
        {
            return await Filter(membership).OfKey(id).ProjectTo<OrganizationCipher>(MapperProvider).SingleOrDefaultAsync();            
        }

        public async Task<EFModel.Cipher> GetEntityAsync(Guid id, OrganizationMembership membership)
        {
            return await Filter(membership).OfKey(id).SingleOrDefaultAsync();            
        }

        public async Task<ICollection<OrganizationCipher>> GetManyAsync(OrganizationMembership membership)
        {
             return await Filter(membership).ProjectTo<OrganizationCipher>(MapperProvider).ToListAsync();            
        }

        public async Task<ICollection<OrganizationCipher>> GetManyAsync(IEnumerable<OrganizationMembership> memberships)
        {
            var list = new List<OrganizationCipher>();
            foreach (var membership in memberships)
            {
                list.AddRange(await GetManyAsync(membership));                
            }
            return list;
        }
        public async Task DeleteAsync(Guid id, OrganizationMembership membership)
        {
            var cipher = await Filter(membership).OfKey(id).SingleOrDefaultAsync();
            dbSet.Remove(cipher);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteManyAsync(IEnumerable<Guid> ids, OrganizationMembership membership)
        {            
            var ciphers = Filter(membership,true).GetMany(ids);
            dbSet.RemoveRange(ciphers);
            await dbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteManyAsync(IEnumerable<Guid> ids, OrganizationMembership membership)
        {
            var ciphers = Filter(membership,true).GetMany(ids);
            ciphers.ToList().ForEach(x=>x.DeletedDate=DateTime.UtcNow);
            await dbContext.SaveChangesAsync();
        }

        public async Task PurgeAsync(OrganizationMembership membership)
        {
            if (membership.Type!= OrganizationUserType.Owner)
                    throw new ForbidException();

            var ciphers = Filter(membership,true).ToList();
            dbSet.RemoveRange(ciphers);
            await dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<OrganizationCipher> ciphers)
        {
            if (!ciphers.Any())
                return;

            var efCiphers = ciphers.Select(x => Mapper.Map<EFModel.Cipher>(x));
            //var efCollections = collections.Select(x => Mapper.Map<EFModel.Collection>(x));

            dbSet.AddRange(efCiphers);
            //dbContext.Collections.AddRange(efCollections);
            await SaveChangesAsync();
        }

        internal IQueryable<EFModel.Cipher> Filter(OrganizationMembership membership,bool managePermission=false)
        {
            if (membership.Type== OrganizationUserType.User && managePermission)
                    return dbSet.Where(x=>false);

            switch (membership.Type)
            {
                case Bit.Core.Enums.OrganizationUserType.Owner:
                case Bit.Core.Enums.OrganizationUserType.Admin:
                    return dbSet.OfOrganization(membership.OrganizationId);
                
                case Bit.Core.Enums.OrganizationUserType.User:                    
                case Bit.Core.Enums.OrganizationUserType.Manager:
                    // TODO : test only
                    return dbSet.OfOrganization(membership.OrganizationId);
                    var allocatedCollectionIds = dbContext.CollectionUsers.OfMembership(membership).Select(x => x.CollectionId).ToList();
                    return dbSet.OfOrganization(membership.OrganizationId).OfCollections(allocatedCollectionIds);

                default:
                    throw new ArgumentException($"bad user type {membership.Type}");                    
            }            
        } 

    }
    static internal class OrganizationCipherQueryExtension
    {       
        static internal IQueryable<T> OfKey<T, TKey>(this IQueryable<T> query, TKey id) where T : IKey<TKey> where TKey : IEquatable<TKey>
            =>  query.Where(x => x.Id.Equals(id));

        static internal IQueryable<EFModel.Cipher> OfOrganization(this IQueryable<EFModel.Cipher> ciphers, Guid organizationId)
            => ciphers.Where(x => x.OrganizationId == organizationId);
        static internal IQueryable<EFModel.Cipher> OfCollections(this IQueryable<EFModel.Cipher> ciphers, IEnumerable<Guid> collectionIds)
            => ciphers.Where(x => collectionIds.Contains(x.CollectionId.Value));

        static internal IQueryable<EFModel.Cipher> GetMany(this IQueryable<EFModel.Cipher> ciphers, IEnumerable<Guid> ids)
            => ciphers.Where(x => ids.Contains(x.Id));
    }

    static internal class CollectionUserQueryExtension
    {
        static internal IQueryable<EFModel.CollectionUser> OfMembership(this IQueryable<EFModel.CollectionUser> ciphers, OrganizationMembership membership)
            => ciphers.Where(x => x.Collection.OrganizationId == membership.OrganizationId && x.OrganizationUser.UserId == membership.UserId);
    }
    static internal class CollectionCipherQueryExtension
    {
        static internal IQueryable<EFModel.CollectionCipher> OfCollection(this IQueryable<EFModel.CollectionCipher> ciphers, Guid collectionId)
            => ciphers.Where(x => x.Collection.Id == collectionId);
    }
}

