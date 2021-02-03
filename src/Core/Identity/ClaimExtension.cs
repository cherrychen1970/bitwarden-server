using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using Bit.Core.Models.Table;
using Bit.Core.Enums;
using OtpNet;
using Bit.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bit.Core.Identity
{
    public static class ClaimExtension
    {
        public static string GetClaim(this ClaimsPrincipal claims,string claimType)
        {            
            var nameClaim = claims.FindFirst(claimType);
            return nameClaim!=null?nameClaim.Value:null;
        }  
        public static string GetName(this ClaimsPrincipal claims) => claims.GetClaim(JwtClaimTypes.Name);
        public static string GetEmailAddress(this ClaimsPrincipal claims) => claims.GetClaim(JwtClaimTypes.Email);
    }
}