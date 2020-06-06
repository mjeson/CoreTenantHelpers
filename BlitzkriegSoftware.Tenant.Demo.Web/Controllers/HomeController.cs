using BlitzkriegSoftware.Tenant.Demo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlitzkriegSoftware.Tenant.Demo.Web.Controllers
{
    [Authorize]
    public class HomeController : _CoreTenantControllerBase
    {
        public HomeController(ILogger<_CoreTenantControllerBase> logger) : base(logger) { }
        
        public IActionResult Index()
        {
            return this.View(this.TenantUser);
        }

        public IActionResult AadInfo()
        {
            return this.View(this.User);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

    }
}
