using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/proxy")]
public class ProxyController : ControllerBase
{
    private readonly IProxyService _proxyService;

    public ProxyController(IProxyService proxyService)
    {
        _proxyService = proxyService;
    }


    // {ANY METHOD}: {anyPath}?{anyQuery}

    [HttpGet("{**path}"), HttpPost("{**path}"), HttpDelete("{**path}"), HttpPut("{**path}"), HttpPatch("{**path}")]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Forward(string path, [FromBody] JsonElement? raw, CancellationToken ct = default)
    {
        string? body = raw.HasValue &&
              raw.Value.ValueKind != JsonValueKind.Null &&
              raw.Value.ValueKind != JsonValueKind.Undefined
        ? raw.Value.GetRawText()
        : null;

        var response = await _proxyService.ForwardAsync(Request, path, body, ct);

        var content = await response.Content.ReadAsStringAsync();
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = contentType
        };
    }
}
