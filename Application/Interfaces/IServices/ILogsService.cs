using Domain.Enums;
using System.Diagnostics;
using Application.Common;
using Application.Common.Dto;

namespace Application.Interfaces.IServices;

public interface ILogsService
{
    /// <summary>
    /// Gets specified number of logs for specified page
    /// </summary>
    /// <param name="q">Query params</param>
    /// <returns>Paged result of Log Dtos</returns>
    Task<PagedResult<LogDto>> GetLogsAsync(QueryDto q, CancellationToken ct);

    /// <summary>
    /// Gets specified number of problems for specified page
    /// </summary>
    /// <param name="q">Query params</param>
    /// <returns>Paged result of Problem Dtos</returns>
    Task<PagedResult<ProblemDto>> GetProblemsAsync(QueryDto q, CancellationToken ct);

    /// <summary>
    /// Adds a log to database
    /// </summary>
    /// <param name="req">Http Request Message</param>
    /// <param name="res">Http Response Message</param>
    /// <param name="sw">Stopwatch for diagnostics</param>
    /// <returns>Created log</returns>
    Task<LogDto> AddLogAsync(HttpRequestMessage req, HttpResponseMessage res, Stopwatch sw, CancellationToken ct);
}
