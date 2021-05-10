using System;
using Bit.Core.Enums;
using Bit.Core.Models.Data;
using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models;
using Bit.Core.Utilities;
namespace Bit.Core.Models.Api
{
    public class OrganizationUserResponseModel : ResponseModel
    {
        public OrganizationUserResponseModel(OrganizationMembershipProfile organizationUser, string obj = "organizationUser")
            : base(obj)
        {
            if (organizationUser == null)
            {
                throw new ArgumentNullException(nameof(organizationUser));
            }

            Id = organizationUser.Id;
            UserId = organizationUser.UserId;
            Name = organizationUser.UserName;
            Type = organizationUser.Type;
            Status = organizationUser.Status;
            AccessAll = organizationUser.AccessAll;
            //Permissions = CoreHelpers.LoadClassFromJsonData<Permissions>(organizationUser.Permissions);
        }

        public OrganizationUserResponseModel(OrganizationMembershipProfile organizationUser,
            IEnumerable<CollectionAssigned> collections)
            : this(organizationUser, "organizationUserDetails")
        {
            Collections = collections.Select(c => new CollectionUserResponseModel(c));
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name {get;set;}
        public OrganizationUserType Type { get; set; }
        public OrganizationUserStatusType Status { get; set; }
        public bool AccessAll { get; set; }
        public Permissions Permissions { get; set; }
        public IEnumerable<CollectionUserResponseModel> Collections { get; set; }
    }
}
