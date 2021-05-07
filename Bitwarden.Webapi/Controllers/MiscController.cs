using System;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Models.Api;
using System.Threading.Tasks;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Bit.Core;
using Stripe;
using System.Linq;
using System.Collections.Generic;

namespace Bit.Api.Controllers
{
    public class MiscController : Controller
    {        
        private readonly GlobalSettings _globalSettings;

        public MiscController(
     
            GlobalSettings globalSettings)
        {
                 _globalSettings = globalSettings;
        }

        [HttpGet("~/alive")]
        [HttpGet("~/now")]
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }

        [HttpGet("~/version")]
        public VersionResponseModel Version()
        {
            return new VersionResponseModel();
        }

        [HttpGet("~/ip")]
        public JsonResult Ip()
        {
            var headerSet = new HashSet<string> { "x-forwarded-for", "cf-connecting-ip", "client-ip" };
            var headers = HttpContext.Request?.Headers
                .Where(h => headerSet.Contains(h.Key.ToLower()))
                .ToDictionary(h => h.Key);
            return new JsonResult(new
            {
                Ip = HttpContext.Connection?.RemoteIpAddress?.ToString(),
                Headers = headers,
            });
        }
    }
}
