namespace Application.Interfaces.IClients;

public interface IApiClient
{
    /// <summary>
    /// Sends a valid request to the original API based on the method
    /// </summary>
    /// <param name="req">HTTP Request Message</param>
    /// <returns>The actual HTTP Response from the original API</returns>
    Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage req, CancellationToken ct);
}
