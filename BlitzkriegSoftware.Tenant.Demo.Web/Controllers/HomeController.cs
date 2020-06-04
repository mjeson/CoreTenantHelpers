using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlitzkriegSoftware.Tenant.Demo.Web.Models;
using MongoDB.Libmongocrypt;

namespace BlitzkriegSoftware.Tenant.Demo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this._logger = logger;
        }

        public IActionResult Index()
        {
            var username = this.User.Identity.Name;
            username = BlitzkriegSoftware.Tenant.Demo.Web.Libs.ParseName.RemoveParts(this.User.Identity.Name,  BlitzkriegSoftware.Tenant.Demo.Web.Libs.ParseParts.PoundPart);
            var model = Program.UserProvider.Read(username);
            this.Enrich(ref model);
            return this.View(model);
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

        private static string[] okKeys = new string[] { "emailaddress", "surname", "givenname" }; 

        private void Enrich(ref BlitzkriegSoftware.Tenant.Libary.Models.TenantUserProfileBase model)
        {
            foreach (var c in this.User.Claims)
            {
                var key = c.Type;
                var index = key.LastIndexOf('/');
                if (index >= 0) key = key.Substring(index + 1);
                var value = c.Value;
                if(okKeys.Contains(key))
                {
                    model.SettingsPut(key, value);
                }
            }
        }
        
    }
}
