using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SalesApp.Controllers
{
    public class SessionController : Controller
    {
        // GET: /<controller>/
        public IActionResult Refresh()
        {
            // TODO: Fix; somehow challenge is currently redirecting to Cookie Middleware's Access Denied page...
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            //HttpContext.Authentication.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties(), Microsoft.AspNetCore.Http.Features.Authentication.ChallengeBehavior.Automatic);
        }
    }
}
