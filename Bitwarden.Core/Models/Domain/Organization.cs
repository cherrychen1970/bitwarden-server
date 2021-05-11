using System;
using Bit.Core.Utilities;
using Bit.Core.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Bit.Core.Models
{
    public class Organization : BaseModel, IReferenceable
    {
        private Dictionary<TwoFactorProviderType, TwoFactorProvider> _twoFactorProviders;        
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress1 { get; set; }
        public string BusinessAddress2 { get; set; }
        public string BusinessAddress3 { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessTaxNumber { get; set; }
        public string BillingEmail { get; set; }
        public string Plan { get; set; } = "Free";
        public PlanType PlanType { get; set; } = PlanType.Free;
        public short? Seats { get; set; }=999;
        public short? MaxCollections { get; set; }
        public bool UsePolicies { get; set; }
        public bool UseSso { get; set; }
        public bool UseGroups { get; set; }
        public bool UseDirectory { get; set; }
        public bool UseEvents { get; set; }
        public bool UseTotp { get; set; }
        public bool Use2fa { get; set; }
        public bool UseApi { get; set; }
        public bool SelfHost { get; set; }=true;
        public bool UsersGetPremium { get; set; }
        public long? Storage { get; set; }=null;
        public short? MaxStorageGb { get; set; }=null;
        public GatewayType? Gateway { get; set; } = null;
        public string GatewayCustomerId { get; set; } = null;
        public string GatewaySubscriptionId { get; set; } =null;
        public string ReferenceData { get; set; }
        public bool Enabled { get; set; } = true;
        public string LicenseKey { get; set; }=CoreHelpers.SecureRandomString(20);
        public string ApiKey { get; set; }=CoreHelpers.SecureRandomString(30);
        public string TwoFactorProviders { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Enums.ReferenceEventSource source => Enums.ReferenceEventSource.Organization;
        public void SetNewId() =>       
            Id = Guid.NewGuid();
    }
}
