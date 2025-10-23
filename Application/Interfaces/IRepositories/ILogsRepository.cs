using Domain.Enums;
using Domain.Entities;

namespace Application.Interfaces.IRepositories;

public interface ILogsRepository
{
    /// <summary>
    /// Adds a log in the database
    /// </summary>
    /// <param name="log">Log object to be added</param>
    /// <returns>Added log object</returns>
    Task<Log> AddLogAsync(Log log);

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
    /// Checks if any changes are saved to the database
    /// </summary>
    /// <returns>True if changes are saved, false if not</returns>
    Task<bool> IsSavedAsync();
}
