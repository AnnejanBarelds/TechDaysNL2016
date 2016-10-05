using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SalesApp.Controllers
{
    [Authorize(Roles = "Order_management")]
    public class APIController : Controller
    {
        private TokenHelper _tokenHelper;

        public APIController(TokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        // GET: /<controller>/
        public async Task<ActionResult> PlaceOrder()
        {
            var url = GetOrderUrl() + "/api/order/placeorder";
            var token = await _tokenHelper.RetrieveToken(Startup.OrderServiceResourceId);

            return await ExecuteCall(url, token);
        }

        public async Task<ActionResult> PlaceOrderV2()
        {
            var url = GetOrderUrl() + "/api/order/placeorderv2";
            var token = await _tokenHelper.RetrieveToken(Startup.OrderServiceResourceId);

            return await ExecuteCall(url, token);
        }

        public async Task<ActionResult> ReadInventory()
        {
            var url = GetInventoryUrl() + "/api/inventory/read";
            var token = await _tokenHelper.RetrieveToken(Startup.InventoryServiceResourceId);

            return await ExecuteCall(url, token);
        }

        public async Task<ActionResult> WriteInventory()
        {
            var url = GetInventoryUrl() + "/api/inventory/write";
            var token = await _tokenHelper.RetrieveToken(Startup.InventoryServiceResourceId);

            return await ExecuteCall(url, token);
        }

        private async Task<ActionResult> ExecuteCall(string url, string token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseToken = await response.Content.ReadAsStringAsync();
                ViewBag.StatusCode = response.StatusCode;
                ViewBag.Token = _tokenHelper.TokenPrettify(responseToken);
                return View("Claims");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var responseToken = await response.Content.ReadAsStringAsync();
                ViewBag.StatusCode = response.StatusCode;
                ViewBag.Token = _tokenHelper.TokenPrettify(responseToken);
                return View("Claims");
            }
            else
            {
                return View("Error");
            }
        }

        private string GetInventoryUrl()
        {
            return "https://localhost:44321";
        }

        private string GetOrderUrl()
        {
            return "https://localhost:44346";
        }
    }
}
