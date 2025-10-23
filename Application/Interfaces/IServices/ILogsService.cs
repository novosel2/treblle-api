using Domain.Enums;
using Domain.Entities;
using System.Diagnostics;
using Application.Common;
using Application.Common.Dto;

namespace Application.Interfaces.IServices;

public interface ILogsService
{
    /// <summary>
    /// Gets specified number of logs for specified page
    /// </summary>
    /// <param name="page">Pagination for logs</param>
    /// <param name="limit">Number of logs</param>
    /// <param name="sortBy">Sort by this property</param>
    /// <param name="sortDir">Sort direction</param>
    /// <param name="methods">Filter by these methods</param>
    /// <param name="statusCodes">Filter by these status codes</param>
    /// <param name="responseLte">Filter logs with response time less than this number</param>
    /// <param name="responseGte">Filter logs with response time more than this number</param>
    /// <param name="createdFrom">Filter logs with date after this date</param>
    /// <param name="createdTo">Filter logs with date before this date</param>
    /// <param name="search">Search query</param>
    /// <returns>Paged result of Log Dtos</returns>
    Task<PagedResult<LogDto>> GetLogsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLte, double? responseGte,
            DateTimeOffset? createdFrom, DateTimeOffset? createdTo, string? search, CancellationToken ct = default);

    /// <summary>
    /// Gets specified number of problems for specified page
    /// </summary>
    /// <param name="page">Pagination for problems</param>
    /// <param name="limit">Number of problems</param>
    /// <param name="sortBy">Sort by this property</param>
    /// <param name="sortDir">Sort direction</param>
    /// <param name="methods">Filter by these methods</param>
    /// <param name="statusCodes">Filter by these status codes</param>
    /// <param name="responseLte">Filter problems with response time less than this number</param>
    /// <param name="responseGte">Filter problems with response time more than this number</param>
    /// <param name="createdFrom">Filter problems with date after this date</param>
    /// <param name="createdTo">Filter problems with date before this date</param>
    /// <returns>Paged result of Problem Dtos</returns>
    Task<PagedResult<ProblemDto>> GetProblemsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLte, double? responseGte,
            DateTimeOffset? createdFrom, DateTimeOffset? createdTo, CancellationToken ct);

    /// <summary>
    /// Adds a log to database
    /// </summary>
    /// <param name="req">Http Request Message</param>
    /// <param name="res">Http Response Message</param>
    /// <param name="sw">Stopwatch for diagnostics</param>
    /// <returns>Created log</returns>
    Task<LogDto> AddLogAsync(HttpRequestMessage req, HttpResponseMessage res, Stopwatch sw, CancellationToken ct);
}
