using System.Security.Claims;
using IdentityModel;

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