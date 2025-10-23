using Application.Interfaces.IClients;

namespace Infrastructure.Clients;

public class ApiClient : IApiClient
{
    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage req)
    {
        using (var client = new HttpClient())
        {
            HttpResponseMessage response = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
            return response;
        }
    }
}
