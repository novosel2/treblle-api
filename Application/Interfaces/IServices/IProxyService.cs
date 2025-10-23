using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.IServices;

public interface IProxyService
{
    /// <summary>
    /// Forwards a request to the original API and logs important data
    /// </summary>
    /// <param name="incomingRequest">Incoming HTTP Request data</param>
    /// <param name="targetPath">Target path for the original API</param>
    /// <param name="body">Body of the request</param>
    /// <returns>Actual response from the original API</returns>
    Task<HttpResponseMessage> ForwardAsync(HttpRequest incomingRequest, string targetPath, string? body, CancellationToken ct);
}
