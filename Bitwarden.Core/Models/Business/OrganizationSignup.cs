using Bit.Core.Enums;
using Bit.Core.Models;

namespace Bit.Core.Models.Business
{
    public class OrganizationSignup 
    {
        public string Name { get; set; }
        public string BillingEmail { get; set; }
        public User Owner { get; set; }
        public string OwnerKey { get; set; }
        public string CollectionName { get; set; }
        public string BusinessName { get; set; }
        public PlanType Plan { get; set; }
    }
}
