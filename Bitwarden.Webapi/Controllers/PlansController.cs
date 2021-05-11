using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Utilities;
using Bit.Core.Models.StaticStore;
using Bit.Core.Models.Api;
using Bit.Core.Enums;
using System.Linq;
using Bit.Core.Repositories;
using System.Threading.Tasks;

namespace Bit.Api.Controllers
{
    [Route("api/plans")]
    [Authorize("Web")]
    public class PlansController : Controller
    {
        
        public PlansController()
        {            
        }

        [HttpGet("")]
        public ListResponseModel<PlanResponseModel> Get()
        {
            //var data = StaticStore.Plans;
            var data = new List<Plan>
            {
                new Plan
                {
                    Type = PlanType.Free,
                    Product = ProductType.Free,
                    Name = "Free",
                    NameLocalizationKey = "planNameFree",
                    DescriptionLocalizationKey = "planDescFree",
                    BaseSeats = 0,
                    //MaxCollections = 2,
                    //MaxUsers = 2,
                    //HasPolicies = true,
                    HasGroups = true,
                    HasDirectory = true,
                    //HasEvents = true,
                    //HasTotp = true,
                    //Has2fa = true,
                    HasApi = true,
                    HasSelfHost = true,
                    HasSso = true,
                    UpgradeSortOrder = -1, // Always the lowest plan, cannot be upgraded to
                    DisplaySortOrder = -1
                }
            };
            var responses = data.Select(plan => new PlanResponseModel(plan));
            return new ListResponseModel<PlanResponseModel>(responses);
        }
    }
}
