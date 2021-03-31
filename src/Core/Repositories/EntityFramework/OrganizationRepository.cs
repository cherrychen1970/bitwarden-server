using System;
using System.Threading.Tasks;
using TableModel = Bit.Core.Models.Table;
using DataModel = Bit.Core.Models.Data;
using EFModel = Bit.Core.Models.EntityFramework;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Bit.Core.Models.Table;

namespace Bit.Core.Repositories.EntityFramework
{
    public class OrganizationRepository : Repository<TableModel.Organization, EFModel.Organization, Guid>, IOrganizationRepository
    {
        public OrganizationRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<Organization> GetByIdentifierAsync(string identifier)
        {
            var organization = await dbSet.Where(e => e.Identifier == identifier)
                .FirstOrDefaultAsync();
            return organization;
        }

        public async Task<ICollection<TableModel.Organization>> GetManyByEnabledAsync()
        {
            var organizations = await dbSet.Where(e => e.Enabled).ToListAsync();
            return Mapper.Map<List<TableModel.Organization>>(organizations);
        }

        public async Task<ICollection<TableModel.Organization>> GetManyByUserIdAsync(Guid userId)
        {
            // TODO
            return await Task.FromResult(null as ICollection<TableModel.Organization>);
        }

        public async Task<ICollection<TableModel.Organization>> SearchAsync(string name, string userEmail, bool? paid,
            int skip, int take)
        {
            // TODO: more filters
            var organizations = await dbSet
            .Where(e => name == null || e.Name.StartsWith(name))
            .OrderBy(e => e.Name)
            .Skip(skip).Take(take)
            .ToListAsync();
            return Mapper.Map<List<TableModel.Organization>>(organizations);

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
                Id = e.Id,
                Use2fa = e.Use2fa,
                UseEvents = e.UseEvents,
                UsersGetPremium = e.UsersGetPremium,
                Using2fa = e.Use2fa && e.TwoFactorProviders != null,
                UseSso = e.UseSso,
            }).ToListAsync();

        }
    }
}
