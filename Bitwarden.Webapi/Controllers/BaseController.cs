using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Bit.Api.Controllers
{    
    [Authorize("Application")]
    public class BaseController : Controller
    {
    }
}