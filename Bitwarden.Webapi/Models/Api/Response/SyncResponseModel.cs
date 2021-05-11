using System;
using System.Collections.Generic;
using System.Linq;
using Bit.Core.Models.Data;
using Bit.Core.Models;

namespace Bit.Core.Models.Api
{
    public class SyncResponseModel : ResponseModel
    {
        public SyncResponseModel(
            GlobalSettings globalSettings,
            User user,
            bool userTwoFactorEnabled,
            IEnumerable<OrganizationMembershipProfile> organizationUsers,
            IEnumerable<Folder> folders,
            IEnumerable<Collection> collections,
            IEnumerable<UserCipher> ciphers,
            IEnumerable<OrganizationCipher> orgCiphers,
            IDictionary<Guid, IGrouping<Guid, CollectionCipher>> collectionCiphersDict,
            bool excludeDomains,
            IEnumerable<Policy> policies
            )
            : base("sync")
        {
            Profile = new ProfileResponseModel(user, organizationUsers, userTwoFactorEnabled);
            Folders = folders.Select(f => new FolderResponseModel(f));
            var list = new List<CipherDetailsResponseModel>();
            list.AddRange(ciphers.Select(c => new CipherDetailsResponseModel(c)));
            list.AddRange(orgCiphers.Select(c => new CipherDetailsResponseModel(c)));
            Ciphers = list.ToArray();
            Collections = collections?.Select(
                c => new CollectionDetailsResponseModel(c)) ?? new List<CollectionDetailsResponseModel>();
            Domains = excludeDomains ? null : new DomainsResponseModel(user, false);
            Policies = policies?.Select(p => new PolicyResponseModel(p)) ?? new List<PolicyResponseModel>();            
        }

        public ProfileResponseModel Profile { get; set; }
        public IEnumerable<FolderResponseModel> Folders { get; set; }
        public IEnumerable<CollectionDetailsResponseModel> Collections { get; set; }
        public IEnumerable<CipherDetailsResponseModel> Ciphers { get; set; }
        public DomainsResponseModel Domains { get; set; }
        public IEnumerable<PolicyResponseModel> Policies { get; set; }        
    }
}
