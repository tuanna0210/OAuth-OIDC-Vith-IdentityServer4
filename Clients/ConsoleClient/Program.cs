using IdentityModel.Client;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var _discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5551");

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,

                ClientId = "console-client",
                ClientSecret = "console-client-secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
        }
    }
}