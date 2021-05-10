using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models.Data;

namespace Bit.Core.Models.Business
{
    public class OrganizationUserInvite
    {
        public IEnumerable<string> Emails { get; set; }
        public Enums.OrganizationUserType Type { get; set; }
        public bool AccessAll { get; set; }
        public Permissions Permissions { get; set; }
        public IEnumerable<CollectionAssigned> Collections { get; set; }
        public OrganizationUserInvite() {}
    }
}
