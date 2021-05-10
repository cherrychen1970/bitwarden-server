using System;
using System.Threading.Tasks;
using Bit.Core.Models;
using Bit.Core.Enums;
using Microsoft.AspNetCore.Http;
using Bit.Core.Models;
using System.Net.Http;
using Bit.Core.Models.Api;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Bit.Core.Repositories;

namespace Bit.Core.Services
{
    public class RelayPushNotificationService : BaseIdentityClientService, IPushNotificationService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RelayPushNotificationService> _logger;

        public RelayPushNotificationService(
            IDeviceRepository deviceRepository,
            GlobalSettings globalSettings,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RelayPushNotificationService> logger)
            : base(
                  globalSettings.PushRelayBaseUri,
                  globalSettings.Installation.IdentityUri,
                  "api.push",
                  $"installation.{globalSettings.Installation.Id}",
                  globalSettings.Installation.Key,
                  logger)
        {
            _deviceRepository = deviceRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task PushSyncCipherCreateAsync(UserCipher cipher)
        {
            await PushCipherAsync(cipher, PushType.SyncCipherCreate);
        }

        public async Task PushSyncCipherUpdateAsync(UserCipher cipher)
        {
            await PushCipherAsync(cipher, PushType.SyncCipherUpdate);
        }

        public async Task PushSyncCipherDeleteAsync(UserCipher cipher)
        {
            await PushCipherAsync(cipher, PushType.SyncLoginDelete);
        }

        private async Task PushCipherAsync(UserCipher cipher, PushType type)
        {
            var message = new SyncCipherPushNotification
            {
                Id = cipher.Id,
                UserId = cipher.UserId,
                RevisionDate = cipher.RevisionDate,
            };

            await SendPayloadToUserAsync(cipher.UserId, type, message, true);
        }

        public async Task PushSyncFolderCreateAsync(Folder folder)
        {
            await PushFolderAsync(folder, PushType.SyncFolderCreate);
        }

        public async Task PushSyncFolderUpdateAsync(Folder folder)
        {
            await PushFolderAsync(folder, PushType.SyncFolderUpdate);
        }

        public async Task PushSyncFolderDeleteAsync(Folder folder)
        {
            await PushFolderAsync(folder, PushType.SyncFolderDelete);
        }

        private async Task PushFolderAsync(Folder folder, PushType type)
        {
            var message = new SyncFolderPushNotification
            {
                Id = folder.Id,
                UserId = folder.UserId,
                RevisionDate = folder.RevisionDate
            };

            await SendPayloadToUserAsync(folder.UserId, type, message, true);
        }

        public async Task PushSyncCiphersAsync(Guid userId)
        {
            await PushUserAsync(userId, PushType.SyncCiphers);
        }

        public async Task PushSyncVaultAsync(Guid userId)
        {
            await PushUserAsync(userId, PushType.SyncVault);
        }

        public async Task PushSyncOrgKeysAsync(Guid userId)
        {
            await PushUserAsync(userId, PushType.SyncOrgKeys);
        }

        public async Task PushSyncSettingsAsync(Guid userId)
        {
            await PushUserAsync(userId, PushType.SyncSettings);
        }

        public async Task PushLogOutAsync(Guid userId)
        {
            await PushUserAsync(userId, PushType.LogOut);
        }

        private async Task PushUserAsync(Guid userId, PushType type)
        {
            var message = new UserPushNotification
            {
                UserId = userId,
                Date = DateTime.UtcNow
            };

            await SendPayloadToUserAsync(userId, type, message, false);
        }

        private async Task SendPayloadToUserAsync(Guid userId, PushType type, object payload, bool excludeCurrentContext)
        {
            throw new System.NotSupportedException();
            /*
            var request = new PushSendRequestModel
            {
                UserId = userId.ToString(),
                Type = type,
                Payload = payload
            };

            await AddCurrentContextAsync(request, excludeCurrentContext);
            await SendAsync(HttpMethod.Post, "push/send", request);
            */
        }

        public Task SendPayloadToUserAsync(string userId, PushType type, object payload, string identifier,
            string deviceId = null)
        {
            throw new NotImplementedException();
        }

        public Task SendPayloadToOrganizationAsync(string orgId, PushType type, object payload, string identifier,
            string deviceId = null)
        {
            throw new NotImplementedException();
        }
    }
}
