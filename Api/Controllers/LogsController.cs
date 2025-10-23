using Application.Enums;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogsService _logsService;

    public LogsController(ILogsService logsService)
    {
        _logsService = logsService;
    }


    // GET: api/logs?page=0&limit=20&sortBy=createdAt&sortDir=desc&method=GET&method=POST...

    [HttpGet]
    public async Task<IActionResult> GetLogs(
            [FromQuery] int page = 0,
            [FromQuery] int limit = 20,
            [FromQuery] SortByEnum sortBy = SortByEnum.CreatedAt, // createdAt, responseTime, statusCode
            [FromQuery] SortDirEnum sortDir = SortDirEnum.Desc, // desc, asc
            [FromQuery] MethodsEnum[]? method = null, // GET, POST, PUT, DELETE
            [FromQuery] int[]? statusCode = null, // 200, 204, 400, 401, 500...
            [FromQuery] double? responseLessThan = null, // 100, 200, 300 (ms)
            [FromQuery] double? responseMoreThan = null, // 100, 200, 300 (ms)
            [FromQuery] DateTime? createdFrom = null, // 2025-11-06
            [FromQuery] DateTime? createdTo = null // 2025-12-01
            )
    {
        var logs = await _logsService.GetLogsAsync(page, limit, sortBy, sortDir, method, statusCode,
                responseLessThan, responseMoreThan, createdFrom, createdTo);
        return Ok(logs);
    }
}
