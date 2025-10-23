using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/proxy")]
public class ProxyController : ControllerBase
{
    public ProxyController()
    {

    }


    // GET: {anyPath}?{anyQuery}

    [HttpGet("{**path}"), HttpPost("{**path}"), HttpDelete("{**path}"), HttpPut("{**path}"), HttpPatch("{**path}")]
    public IActionResult Forward(string? path)
    {
        return Ok(path ?? "No path");
    }
}
