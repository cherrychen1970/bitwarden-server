using System;
using Bit.Core.Models.Table;

namespace Bit.Core.Repositories.Noop
{
    public class InstallationRepository : Repository<Installation, Guid>, IInstallationRepository
    {
    }
}
