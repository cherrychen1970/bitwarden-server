using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.Core.Models.Api.Response;
using Bit.Core.Models.Data;
using Bit.Core.Models.Table;
using Bit.Core.Repositories;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace Bit.Core.Services
{
    public class NoopEmergencyAccessService : IEmergencyAccessService
    {
        public NoopEmergencyAccessService()
        {
            throw new NotImplementedException();
        }

        public async Task<EmergencyAccess> InviteAsync(User invitingUser, string email, EmergencyAccessType type, int waitTime)
        {
            throw new NotImplementedException();
        }

        public async Task<EmergencyAccessDetails> GetAsync(Guid emergencyAccessId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task ResendInviteAsync(User invitingUser, Guid emergencyAccessId)
        {
            throw new NotImplementedException();
        }

        public async Task<EmergencyAccess> AcceptUserAsync(Guid emergencyAccessId, User user, string token, IUserService userService)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid emergencyAccessId, Guid grantorId)
        {
            throw new NotImplementedException();
        }

        public async Task<EmergencyAccess> ConfirmUserAsync(Guid emergencyAcccessId, string key, Guid confirmingUserId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(EmergencyAccess emergencyAccess, Guid savingUserId)
        {
            throw new NotImplementedException();
        }

        public async Task InitiateAsync(Guid id, User initiatingUser)
        {
            throw new NotImplementedException();
        }

        public async Task ApproveAsync(Guid id, User approvingUser)
        {
            throw new NotImplementedException();

        }

        public async Task RejectAsync(Guid id, User rejectingUser)
        {
            throw new NotImplementedException();

        }

        public async Task<(EmergencyAccess, User)> TakeoverAsync(Guid id, User requestingUser)
        {
            throw new NotImplementedException();

        }

        public async Task PasswordAsync(Guid id, User requestingUser, string newMasterPasswordHash, string key)
        {
            throw new NotImplementedException();

        }

        public async Task SendNotificationsAsync()
        {
            throw new NotImplementedException();

        }

        public async Task HandleTimedOutRequestsAsync()
        {
            throw new NotImplementedException();

        }

        public async Task<EmergencyAccessViewResponseModel> ViewAsync(Guid id, User requestingUser)
        {
            throw new NotImplementedException();
        }

        private async Task SendInviteAsync(EmergencyAccess emergencyAccess, string invitingUsersName)
        {
            throw new NotImplementedException();

        }

        private string NameOrEmail(User user)
        {
            return string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name;
        }
    }
}
