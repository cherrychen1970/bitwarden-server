﻿using System;
using Bit.Core.Enums;
using Bit.Core.Models.Data;
using Bit.Core.Utilities;
namespace Bit.Core.Models.Api
{
    public class ProfileOrganizationResponseModel : ResponseModel
    {
        public ProfileOrganizationResponseModel(OrganizationMembershipProfile membership)
            : base("profileOrganization")
        {
            Id = membership.OrganizationId;
            Name = membership.OrganizationName;
            /*
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
            MaxStorageGb = organization.MaxStorageGb;
            Enabled = organization.Enabled;
            */
            Key = membership.Key;
            Status = membership.Status;
            Type = membership.Type;
            //Identifier = membership.OrganizationIdentifier;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool UsePolicies { get; set; }
        public bool UseSso { get; set; }
        public bool UseGroups { get; set; }
        public bool UseDirectory { get; set; }
        public bool UseEvents { get; set; }
        public bool UseTotp { get; set; }
        public bool Use2fa { get; set; }
        public bool UseApi { get; set; }
        public bool UseBusinessPortal => UsePolicies || UseSso; // TODO add events if needed
        public bool UsersGetPremium { get; set; }
        public bool SelfHost { get; set; }=true;
        public int Seats { get; set; }
        public int MaxCollections { get; set; }
        public short? MaxStorageGb { get; set; }
        public string Key { get; set; }
        public OrganizationUserStatusType Status { get; set; }
        public OrganizationUserType Type { get; set; }
        public bool Enabled { get; set; }=true;
        public bool SsoBound { get; set; }
        public string Identifier { get; set; } = Guid.Empty.ToString();
        public Permissions Permissions { get; set; }
    }
}
