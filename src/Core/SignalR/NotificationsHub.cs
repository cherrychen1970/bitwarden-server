﻿using System;
using System.Threading.Tasks;
using Bit.Core;
using Microsoft.AspNetCore.Authorization;

namespace Bit.Notifications
{
    [Authorize("Application")]
    public class NotificationsHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ConnectionCounter _connectionCounter;
        private readonly GlobalSettings _globalSettings;
        private readonly ISessionContext _sessionContext;

        public NotificationsHub(ConnectionCounter connectionCounter, ISessionContext sessionContext,  GlobalSettings globalSettings)
        {
            _connectionCounter = connectionCounter;
            _globalSettings = globalSettings;
            _sessionContext = sessionContext;            
        }

        public override async Task OnConnectedAsync()
        {
            if (_sessionContext.HasOrganizations())
            {
                foreach (var org in _sessionContext.Organizations)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Organization_{org.Id}");
                }
            }
            _connectionCounter.Increment();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_sessionContext.HasOrganizations())
            {
                foreach (var org in _sessionContext.Organizations)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Organization_{org.Id}");
                }
            }
            _connectionCounter.Decrement();
            await base.OnDisconnectedAsync(exception);
        }
    }
}