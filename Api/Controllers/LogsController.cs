using Domain.Enums;
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
        if (q.Page < 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid pagination",
                Detail = "Page cannot be less than 0",
                Status = 400
            });
        }

        if (q.Limit <= 0 || q.Limit > 200)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid limit",
                Detail = "Limit must be 1-200",
                Status = 400
            });
        }

        if (q.ResponseGte <= 0 || q.ResponseLte <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid response time",
                Detail = "ResponseGte and ResponseLte cannot be less than 0",
                Status = 400
            });
        }

        if (q.CreatedFrom.HasValue && q.CreatedTo.HasValue && q.CreatedFrom > q.CreatedTo)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "CreatedAt range invalid",
                Detail = "CreatedAtGte cannot be after CreatedAtLte",
                Status = 400
            });
        }

        if (q.ResponseGte.HasValue && q.ResponseLte.HasValue && q.ResponseGte > q.ResponseLte)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Response Time range invalid",
                Detail = "ResponseGte cannot be bigger ResponseLte",
                Status = 400
            });
        }

        var queryDto = new QueryDto(
            q.Page,
            q.Limit,
            q.SortBy,
            q.SortDir,
            q.Method,
            q.StatusCode,
            q.ResponseGte,
            q.ResponseLte,
            q.CreatedFrom,
            q.CreatedTo,
            q.Search
        );

        var pagedResult = await _logsService.GetLogsAsync(queryDto, ct);

        return Ok(pagedResult);
    }


    // GET: api/logs/problems?page=0&limit=20&sortBy=createdAt&sortDir=desc&method=GET&method=POST...

    [HttpGet("problems")]
    [ProducesResponseType(typeof(PagedResult<ProblemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ProblemDto>>> GetProblems([FromQuery] ProblemQueryParams q,
            CancellationToken ct = default)
    {
        if (q.Page < 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid pagination",
                Detail = "Page cannot be less than 0",
                Status = 400
            });

        }

        if (q.Limit < 0 || q.Limit > 200)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid limit",
                Detail = "Limit must be 1-200",
                Status = 400
            });

        }

        if (q.ResponseGte <= 0 || q.ResponseLte <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid response time",
                Detail = "ResponseGte and ResponseLte cannot be less than 0",
                Status = 400
            });

        }

        if (q.CreatedFrom.HasValue && q.CreatedTo.HasValue && q.CreatedFrom > q.CreatedTo)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "CreatedAt range invalid",
                Detail = "CreatedAtGte cannot be after CreatedAtLte",
                Status = 400
            });
        }

        if (q.ResponseGte.HasValue && q.ResponseLte.HasValue && q.ResponseGte > q.ResponseLte)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Response Time range invalid",
                Detail = "ResponseGte cannot be bigger ResponseLte",
                Status = 400
            });
        }

        var queryDto = new QueryDto(
            q.Page,
            q.Limit,
            q.SortBy,
            q.SortDir,
            q.Method,
            q.StatusCode,
            q.ResponseGte,
            q.ResponseLte,
            q.CreatedFrom,
            q.CreatedTo,
            Search: null
        );

        var pagedResult = await _logsService.GetProblemsAsync(queryDto, ct);

        return Ok(pagedResult);
    }
}
