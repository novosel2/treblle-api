using Domain.Enums;
using Application.Common.Dto;
using Application.Common;
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


    // GET: api/logs?page=0&limit=20&sortBy=createdAt&sortDir=desc&method=GET,POST...

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<LogDto>>> GetLogs(
            [FromQuery] int page = 0,
            [FromQuery] int limit = 20,
            [FromQuery] SortByEnum sortBy = SortByEnum.CreatedAt, // createdAt, responseTime, statusCode
            [FromQuery] SortDirEnum sortDir = SortDirEnum.Desc, // desc, asc
            [FromQuery] MethodsEnum[]? method = null, // GET, POST, PUT, DELETE
            [FromQuery] int[]? statusCode = null, // 200, 204, 400, 401, 500...
            [FromQuery(Name = "responseTime[gte]")] double? responseGte = null, // 100, 200, 300 (ms)
            [FromQuery(Name = "responseTime[lte]")] double? responseLte = null, // 100, 200, 300 (ms)
            [FromQuery(Name = "createdAt[gte]")] DateTimeOffset? createdFrom = null, // 2025-11-06T11:30:00
            [FromQuery(Name = "createdAt[lte]")] DateTimeOffset? createdTo = null, // 2025-12-01T12:30:00
            [FromQuery(Name = "q")] string? search = null,
            CancellationToken ct = default
            )
    {
        if (page < 0)
            return BadRequest(new ProblemDetails { Title = "Invalid pagination", Detail = "Page cannot be less than 0", Status = 400 });

        if (limit <= 0 || limit > 200)
            return BadRequest(new ProblemDetails { Title = "Invalid limit", Detail = "Limit must be 1-200", Status = 400 });

        if (responseGte <= 0 || responseLte <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid response time", Detail = "ResponseGte and ResponseLte cannot be less than 0", Status = 400 });

        if (createdFrom.HasValue && createdTo.HasValue && createdFrom > createdTo)
            return BadRequest(new ProblemDetails { Title = "CreatedAt range invalid", Detail = "CreatedAtGte cannot be after CreatedAtLte", Status = 400 });

        if (responseGte.HasValue && responseLte.HasValue && responseGte > responseLte)
            return BadRequest(new ProblemDetails { Title = "Response Time range invalid", Detail = "ResponseGte cannot be bigger ResponseLte", Status = 400 });

        var pagedResult = await _logsService.GetLogsAsync(page, limit, sortBy, sortDir, method, statusCode,
                responseLte, responseGte, createdFrom, createdTo, search, ct);

        return Ok(pagedResult);
    }


    // GET: api/logs/problems?page=0&limit=20&sortBy=createdAt&sortDir=desc&method=GET&method=POST...

    [HttpGet("problems")]
    [ProducesResponseType(typeof(PagedResult<ProblemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProblemDto>>> GetProblems(
            [FromQuery] int page = 0,
            [FromQuery] int limit = 20,
            [FromQuery] SortByEnum sortBy = SortByEnum.CreatedAt, // createdAt, responseTime, statusCode
            [FromQuery] SortDirEnum sortDir = SortDirEnum.Desc, // desc, asc
            [FromQuery] MethodsEnum[]? method = null, // GET, POST, PUT, DELETE
            [FromQuery] int[]? statusCode = null, // 200, 204, 400, 401, 500...
            [FromQuery(Name = "responseTime[gte]")] double? responseGte = null, // 100, 200, 300 (ms)
            [FromQuery(Name = "responseTime[lte]")] double? responseLte = null, // 100, 200, 300 (ms)
            [FromQuery(Name = "createdAt[gte]")] DateTimeOffset? createdFrom = null, // 2025-11-06
            [FromQuery(Name = "createdAt[lte]")] DateTimeOffset? createdTo = null, // 2025-12-01
            CancellationToken ct = default
            )
    {
        if (page < 0)
            return BadRequest(new ProblemDetails { Title = "Invalid pagination", Detail = "Page cannot be less than 0", Status = 400 });

        if (limit < 0)
            return BadRequest(new ProblemDetails { Title = "Invalid limit", Detail = "Limit cannot be less than 0", Status = 400 });

        if (responseGte <= 0 || responseLte <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid response time", Detail = "ResponseGte and ResponseLte cannot be less than 0", Status = 400 });

        if (createdFrom.HasValue && createdTo.HasValue && createdFrom > createdTo)
            return BadRequest(new ProblemDetails { Title = "CreatedAt range invalid", Detail = "CreatedAtGte cannot be after CreatedAtLte", Status = 400 });

        if (responseGte.HasValue && responseLte.HasValue && responseGte > responseLte)
            return BadRequest(new ProblemDetails { Title = "Response Time range invalid", Detail = "ResponseGte cannot be bigger ResponseLte", Status = 400 });

        var pagedResult = await _logsService.GetProblemsAsync(page, limit, sortBy, sortDir, method, statusCode,
                responseLte, responseGte, createdFrom, createdTo, ct);

        return Ok(pagedResult);
    }
}
