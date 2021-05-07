using System;
using Bit.Core.Models;
using Bit.Core.Models.Business;
using Bit.Core.Models.StaticStore;
using System.Linq;
using Bit.Core.Enums;

namespace Bit.Core.Models.Api
{
    public class OrganizationResponseModel : ResponseModel
    {
        public OrganizationResponseModel(Organization organization, string obj = "organization")
            : base(obj)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            Id = organization.Id.ToString();
            Identifier = organization.Identifier;
            Name = organization.Name;
            BusinessName = organization.BusinessName;
            BusinessAddress1 = organization.BusinessAddress1;
            BusinessAddress2 = organization.BusinessAddress2;
            BusinessAddress3 = organization.BusinessAddress3;
            BusinessCountry = organization.BusinessCountry;
            BusinessTaxNumber = organization.BusinessTaxNumber;
            BillingEmail = organization.BillingEmail;
            Plan = new PlanResponseModel(Utilities.StaticStore.Plans.FirstOrDefault(plan => plan.Type == organization.PlanType));
            PlanType = organization.PlanType;
            Seats = organization.Seats;
            MaxCollections = organization.MaxCollections;
            MaxStorageGb = organization.MaxStorageGb;
            UsePolicies = organization.UsePolicies;
            UseSso = organization.UseSso;
            UseGroups = organization.UseGroups;
            UseDirectory = organization.UseDirectory;
            UseEvents = organization.UseEvents;
            UseTotp = organization.UseTotp;
            Use2fa = organization.Use2fa;
            UseApi = organization.UseApi;
            UsersGetPremium = organization.UsersGetPremium;
            SelfHost = organization.SelfHost;
        }

        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress1 { get; set; }
        public string BusinessAddress2 { get; set; }
        public string BusinessAddress3 { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessTaxNumber { get; set; }
        public string BillingEmail { get; set; }
        public PlanResponseModel Plan { get; set; }
        public PlanType PlanType { get; set; }
        public short? Seats { get; set; }
        public short? MaxCollections { get; set; }
        public short? MaxStorageGb { get; set; }
        public bool UsePolicies { get; set; }
        public bool UseSso { get; set; }
        public bool UseGroups { get; set; }
        public bool UseDirectory { get; set; }
        public bool UseEvents { get; set; }
        public bool UseTotp { get; set; }
        public bool Use2fa { get; set; }
        public bool UseApi { get; set; }
        public bool UsersGetPremium { get; set; }
        public bool SelfHost { get; set; }
    }
}
