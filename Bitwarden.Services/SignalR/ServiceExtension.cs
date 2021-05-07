
using System.Collections.Generic;
using System.Globalization;
using Bit.Core;
using Bit.Core.Utilities;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace Bit.Notifications
{
    static public class ServiceExtension
    {
        public static void AddCustomSignalR( this IServiceCollection services)
        {
            var signalRServerBuilder = services.AddSignalR().AddMessagePackProtocol(options =>
            {
                options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
                {
                    MessagePack.Resolvers.ContractlessStandardResolver.Instance
                };
            });
        }
    }
}