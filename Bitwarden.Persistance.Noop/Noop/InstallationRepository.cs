using System;
using Bit.Core.Models;

namespace Bit.Core.Repositories.Noop
{
    public class InstallationRepository : Repository<Installation, Guid>, IInstallationRepository
    {
    }
}
