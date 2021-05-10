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
