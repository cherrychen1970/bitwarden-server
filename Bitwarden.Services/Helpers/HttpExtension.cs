using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Utilities;
using Bit.Core.Models.Data;

namespace Bit.Core.Services
{
    static public class HttpExtension
    {
        private static readonly string RealIp = "X-Real-IP";
        public static string DeviceIdentifier(this HttpContext httpContext)
                => httpContext.Request.Headers["Device-Identifier"];

        static public DeviceType? DeviceType(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("Device-Type") &&
                Enum.TryParse(httpContext.Request.Headers["Device-Type"].ToString(), out DeviceType dType))
                return dType;

            return default(Enums.DeviceType);
        }
        static public string IpAddress(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey(RealIp))
            {
                return httpContext.Request.Headers[RealIp].ToString();
            }

            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }

        static public Guid? UserId(this HttpContext httpContext)
        {
            var sub = httpContext?.User?.FindFirst("sub")?.Value;
            if (sub != null)
            {
                Guid user;
                Guid.TryParse(sub, out user);
                return user;
            }
            return null;
        }
    }
}