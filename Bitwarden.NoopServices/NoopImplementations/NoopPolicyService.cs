using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.Core.Repositories;

namespace Bit.Core.Services
{
    public class NoopPolicyService : IPolicyService
    {
        public NoopPolicyService(
)
        {

        }

        public async Task SaveAsync(Policy policy, IUserService userService, IOrganizationService organizationService,
            Guid? savingUserId)
        {
           throw new NotImplementedException();
        }
    }
}
