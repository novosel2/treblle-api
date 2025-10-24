using Application.Common.Dto;
using Application.Common;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Api.Common;

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
    public async Task<ActionResult<PagedResult<LogDto>>> GetLogs([FromQuery] LogQueryParams q, CancellationToken ct = default)
    {
        var (problemDetails, queryDto) = ValidateAndMap(q);

        if (problemDetails != null)
            return BadRequest(problemDetails);

        var pagedResult = await _logsService.GetLogsAsync(queryDto, ct);

        return Ok(pagedResult);
    }


    // GET: api/logs/problems?page=0&limit=20&sortBy=createdAt&sortDir=desc&method=GET&method=POST...

    [HttpGet("problems")]
    [ProducesResponseType(typeof(PagedResult<ProblemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProblemDto>>> GetProblems([FromQuery] ProblemQueryParams q, CancellationToken ct = default)
    {
        var (problemDetails, queryDto) = ValidateAndMap(q);

        if (problemDetails != null)
            return BadRequest(problemDetails);

        var pagedResult = await _logsService.GetProblemsAsync(queryDto, ct);

        return Ok(pagedResult);
    }

    private static (ProblemDetails? error, QueryDto dto) ValidateAndMap(BaseQueryParams q)
    {
        if (q.CreatedFrom.HasValue && q.CreatedTo.HasValue && q.CreatedFrom > q.CreatedTo)
            return (Problem("CreatedAt range invalid", "createdAt[gte] cannot be after createdAt[lte]"), null!);

        if (q.ResponseGte.HasValue && q.ResponseLte.HasValue && q.ResponseGte > q.ResponseLte)
            return (Problem("Response Time range invalid", "responseTime[gte] cannot be greater than responseTime[lte]"), null!);

        var search = (q as LogQueryParams)?.Search;

        var dto = new QueryDto(
            q.Page, q.Limit, q.SortBy, q.SortDir, q.Method, q.StatusCode,
            q.ResponseGte, q.ResponseLte, q.CreatedFrom, q.CreatedTo, search
        );

        return (null, dto);
    }

    private static ProblemDetails Problem(string title, string detail)
    {
        return new ProblemDetails()
        {
            Title = title,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };
    }
}
