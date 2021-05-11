using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Models.Api;
using Bit.Core.Services;
using Bit.Core.Repositories;
using Bit.Core;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using System.Linq;
using Bit.Core.Models;
using System.Collections.Generic;
using Bit.Core.Models.Data;

namespace Bit.Api.Controllers
{
    [Route("api/sync")]
    [Authorize("Application")]
    public class SyncController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFolderRepository _folderRepository;
        private readonly ICipherRepository _cipherRepository;
        private readonly IOrganizationCipherRepository _orgCipherRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICollectionCipherRepository _collectionCipherRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly ISessionContext _currentContext;
        //private readonly ISendRepository _sendRepository;
        private readonly GlobalSettings _globalSettings;

        public SyncController(
            ISessionContext authorized,
            IUserService userService,
            IFolderRepository folderRepository,
            ICipherRepository cipherRepository,
            IOrganizationCipherRepository orgCipherRepository,
            ICollectionRepository collectionRepository,
            ICollectionCipherRepository collectionCipherRepository,
            IOrganizationUserRepository organizationUserRepository,
            IPolicyRepository policyRepository,
            //ISendRepository sendRepository,
            GlobalSettings globalSettings)
        {
            _currentContext = authorized;
            _userService = userService;
            _folderRepository = folderRepository;
            _cipherRepository = cipherRepository;
            _orgCipherRepository = orgCipherRepository;
            _collectionRepository = collectionRepository;
            _collectionCipherRepository = collectionCipherRepository;
            _organizationUserRepository = organizationUserRepository;
            _policyRepository = policyRepository;
            //_sendRepository = sendRepository;
            _globalSettings = globalSettings;
        }

        [HttpGet("")]
        public async Task<SyncResponseModel> Get([FromQuery] bool excludeDomains = false)
        {
            var user = await _userService.GetUserByIdAsync(_currentContext.UserId);
            if (user == null)
            {
                throw new BadRequestException("User not found.");
            }

            var organizationUsers = await _organizationUserRepository.GetManyByUserAsync(user.Id);
            var hasEnabledOrgs = organizationUsers.Any();
            var folders = await _folderRepository.GetManyByUserIdAsync(user.Id);
            var ciphers = (await _cipherRepository.GetManyAsync(user.Id)).ToList();
            //var sends = await _sendRepository.GetManyByUserIdAsync(user.Id);            
            ICollection<OrganizationCipher> orgCiphers = new List<OrganizationCipher>(); 

            IEnumerable<Collection> collections = null;
            // FIX THIS
            IDictionary<Guid, IGrouping<Guid, CollectionCipher>> collectionCiphersGroupDict = null;
            IEnumerable<Policy> policies = null;
            if (hasEnabledOrgs)
            {                
                orgCiphers = await _orgCipherRepository.GetManyAsync(_currentContext.OrganizationMemberships);              
                collections = await _collectionRepository.GetManyAsync(_currentContext.OrganizationMemberships);
                policies = await _policyRepository.GetManyByUserIdAsync(user.Id);
            }
            
            var response = new SyncResponseModel(_globalSettings, user, false, organizationUsers,
                folders, collections, ciphers, orgCiphers, collectionCiphersGroupDict, excludeDomains, policies);
            return response;
        }
    }
}
