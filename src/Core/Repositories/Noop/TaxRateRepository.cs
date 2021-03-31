using Bit.Core.Models.Table;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace Bit.Core.Repositories.Noop
{
    public class TaxRateRepository : Repository<TaxRate, string>, ITaxRateRepository
    {
        public async Task<ICollection<TaxRate>> SearchAsync(int skip, int count)
        {
            return new TaxRate[] {};
        }
        
        public async Task<ICollection<TaxRate>> GetAllActiveAsync()
        {
            return new TaxRate[] {};

        }

        public async Task ArchiveAsync(TaxRate model)
        {
            throw new System.NotImplementedException();

        }

        public async Task<ICollection<TaxRate>> GetByLocationAsync(TaxRate model)
        {
            return new TaxRate[] {};
        }
    }
}
