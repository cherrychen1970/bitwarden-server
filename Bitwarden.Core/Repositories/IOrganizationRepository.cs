using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models.Data;
using Bit.Core.Models;

namespace Bit.Core.Repositories
{
    public interface IOrganizationRepository : IRepository<Organization, Guid>
    {
        Task<Organization> GetByIdentifierAsync(string identifier);
        //Task<ICollection<Organization>> GetManyAsync(IEnumerable<OrganizationMembership> memberships);
        Task<ICollection<OrganizationAbility>> GetManyAbilitiesAsync();
    }
}
