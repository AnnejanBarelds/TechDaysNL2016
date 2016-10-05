using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;

namespace SalesApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private TokenHelper _tokenHelper;

        public HomeController(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        public IActionResult Index()
        {
            ViewBag.Token = _tokenHelper.TokenPrettify(User.Claims.First(claim => claim.Type == "idtoken").Value);
            return View();
        }

        [Authorize(Roles = "Reporting")]
        public async Task<ActionResult> ListEmployees()
        {
            // Get the AD Graph client
            var client = GetActiveDirectoryClient();

            // Retrieve persons reporting to me
            var users = new List<User>();
            var result = await client.Me.DirectReports.ExecuteAsync();

            await Merge(users, result);
            ViewBag.Employees = string.Join(
                Environment.NewLine, users.Select(user => user.DisplayName).ToArray());
            return View("Employees");

            // Just for fun: this is how you'd query the Graph API for groups the user is a member of
            // var result = await client.Me.MemberOf.ExecuteAsync();
        }

        public IActionResult Error()
        {
            return View();
        }

        private ActiveDirectoryClient GetActiveDirectoryClient()
        {
            // Setup the URI to the Graph API
            Uri baseServiceUri = new Uri(Startup.GraphResourceId);

            // Create client using the base URI, our tenant ID and the token delegate
            ActiveDirectoryClient activeDirectoryClient =
                new ActiveDirectoryClient(new Uri(baseServiceUri, Startup.TenantId),
                    async () => await _tokenHelper.RetrieveToken(Startup.GraphResourceId));
            return activeDirectoryClient;
        }

        private async Task Merge<T>(List<T> items, IPagedCollection<IDirectoryObject> items2)
        {
            items.AddRange(items2.CurrentPage.Where(item => item is T).Cast<T>());
            if (items2.MorePagesAvailable)
            {
                var nextPage = await items2.GetNextPageAsync();
                await Merge(items, nextPage);
            }
        }
    }
}
