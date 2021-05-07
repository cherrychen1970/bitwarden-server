using System;
using System.Threading.Tasks;
using Bit.Core.Jobs;
using Bit.Core.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Bit.Api.Jobs
{
    public class ValidateOrganizationsJob : BaseJob
    {
        

        public ValidateOrganizationsJob(        
            ILogger<ValidateOrganizationsJob> logger)
            : base(logger)
        {
            
        }

        protected async override Task ExecuteJobAsync(IJobExecutionContext context)
        {
            
        }
    }
}
