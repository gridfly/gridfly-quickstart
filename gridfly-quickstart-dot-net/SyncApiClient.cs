using System.IO;
using System.Threading.Tasks;
using Gridfly.Client;

namespace Gridfly.QuickStart
{
    public class SyncApiClient : BaseApiClient
    {
        public SyncApiClient(string clientId, string clientSecret) : base(clientId, clientSecret)
        {
        }

        public async Task<byte[]> Export(Stream htmlStream)
        {
            var client = await Authenticate();
            try
            {
                var response = await client.CreateSyncJobAsync(htmlStream);
                using (var ms = new MemoryStream())
                {
                    await response.Stream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Sync Export Failed. Status: {ex.StatusCode}");
                Console.WriteLine($"Response: {ex.Response}");
                throw;
            }
        }
    }
}
