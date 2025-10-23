using Domain.Enums;
using Domain.Entities;
using System.Diagnostics;

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
    /// <param name="responseLessThan">Filter logs with response time less than this number</param>
    /// <param name="responseMoreThan">Filter logs with response time more than this number</param>
    /// <param name="createdFrom">Filter logs with date after this date</param>
    /// <param name="createdTo">Filter logs with date before this date</param>
    /// <param name="search">Search query</param>
    /// <returns>List of logs</returns>
    Task<List<Log>> GetLogsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLessThan, double? responseMoreThan,
            DateTime? createdFrom, DateTime? createdTo, string? search);

    /// <summary>
    /// Gets specified number of problems for specified page
    /// </summary>
    /// <param name="page">Pagination for problems</param>
    /// <param name="limit">Number of problems</param>
    /// <param name="sortBy">Sort by this property</param>
    /// <param name="sortDir">Sort direction</param>
    /// <param name="methods">Filter by these methods</param>
    /// <param name="statusCodes">Filter by these status codes</param>
    /// <param name="responseLessThan">Filter problems with response time less than this number</param>
    /// <param name="responseMoreThan">Filter problems with response time more than this number</param>
    /// <param name="createdFrom">Filter problems with date after this date</param>
    /// <param name="createdTo">Filter problems with date before this date</param>
    /// <returns>List of problems</returns>
    Task<List<Problem>> GetProblemsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLessThan, double? responseMoreThan,
            DateTime? createdFrom, DateTime? createdTo);

    /// <summary>
    /// Adds a log to database
    /// </summary>
    /// <param name="response">Response that needs to be logged</param>
    /// <returns>Created log</returns>
    Task<Log> AddLogAsync(HttpRequestMessage req, HttpResponseMessage res, Stopwatch sw);
}
