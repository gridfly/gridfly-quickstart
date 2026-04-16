using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Gridfly.Client;

namespace Gridfly.QuickStart
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient HttpClient;
        protected readonly string ClientId;
        protected readonly string ClientSecret;

        protected BaseApiClient(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            HttpClient = new HttpClient();
        }

        protected async Task<GridflyClient> Authenticate()
        {
            var client = new GridflyClient(HttpClient);
            var tokenRequest = new TokenRequest
            {
                Grant_type = "client_credentials",
                Client_id = ClientId,
                Client_secret = ClientSecret
            };
            try
            {
                var tokenResponse = await client.GetAccessTokenAsync(tokenRequest);
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Access_token);
                return client;
            }
            catch (ApiException ex)
            {
                System.Console.WriteLine($"Authentication Failed. Status: {ex.StatusCode}");
                System.Console.WriteLine($"Response: {ex.Response}");
                throw;
            }
        }
    }
}
