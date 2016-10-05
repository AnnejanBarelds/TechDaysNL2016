using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SalesApp
{
    public class TokenHelper
    {
        IHttpContextAccessor _contextAccessor;

        public TokenHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<string> RetrieveToken(string resource)
        {
            // Setup the context to talk to our AD tenant
            AuthenticationContext authContext = new AuthenticationContext(Startup.Authority);

            // Get the logged in user ID
            var context = _contextAccessor.HttpContext;
            var user = context.User;
            var userId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            // Get the credentials for app authentication
            ClientCredential credential = new ClientCredential(Startup.ClientId, Startup.ClientSecret);

            // Get the token
            AuthenticationResult result = await authContext.AcquireTokenSilentAsync(
                resource, credential, new UserIdentifier(userId, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }

        #region Prettifiers
        public string TokenPrettify(string token)
        {
            var i = token.IndexOf("}.{");
            var headerString = token.Substring(0, i + 1);
            var valueString = token.Substring(i + 2);

            var header = JsonPrettify(headerString);
            var value = JsonPrettify(valueString);

            return header + Environment.NewLine + "." + Environment.NewLine + value;
        }

        private string JsonPrettify(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        #endregion
    }
}
