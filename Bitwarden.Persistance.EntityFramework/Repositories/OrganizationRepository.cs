using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using DomainModel = Bit.Core.Models;
using DataModel = Bit.Core.Models.Data;
using Bit.Core.Models;
using Bit.Core.Repositories;
using EFModel = Bit.Core.Entities;

namespace Bit.Infrastructure.EntityFramework
{
    public class OrganizationRepository : Repository<Organization, EFModel.Organization, Guid>, IOrganizationRepository
    {
        public OrganizationRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }


        public async Task<Organization> GetByIdentifierAsync(string identifier)
        {
            return await dbSet.Where(e => e.Identifier == identifier).ProjectTo<DomainModel.Organization>(MapperProvider)
                .FirstOrDefaultAsync();            
        }

        public async Task<ICollection<DomainModel.Organization>> GetManyByEnabledAsync()
        {
            return await dbSet.Where(e => e.Enabled).ProjectTo<DomainModel.Organization>(MapperProvider).ToListAsync();            
        }

        public async Task<ICollection<DomainModel.Organization>> GetManyByUserIdAsync(Guid userId)
        {
            // TODO
            return await Task.FromResult(null as ICollection<DomainModel.Organization>);
        }

        public async Task<ICollection<DomainModel.Organization>> SearchAsync(string name, string userEmail, bool? paid,
            int skip, int take)
        {
            // TODO: more filters
            var organizations = await dbSet
            .Where(e => name == null || e.Name.StartsWith(name))
            .OrderBy(e => e.Name)
            .Skip(skip).Take(take)
            .ToListAsync();
            return Mapper.Map<List<DomainModel.Organization>>(organizations);

        }

        public async Task UpdateStorageAsync(Guid id)
        {
            await Task.CompletedTask;
            return;
        }

        public async Task<ICollection<DataModel.OrganizationAbility>> GetManyAbilitiesAsync()
        {
            return await dbSet
            .Select(e => new DataModel.OrganizationAbility
            {
                Enabled = e.Enabled,
                Id = e.Id
            }).ToListAsync();

        }
    }
}
