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
    /// Checks if the same problem already occurred in the last 20 minutes
    /// </summary>
    /// <param name="type">Problem type that occurred</param>
    /// <param name="path">Path with the problem occurrence</param>
    /// <param name="method">Method of the request</param>
    /// <returns>True if problem already occurred, false if not</returns>
    Task<Problem?> GetExistingProblemAsync(ProblemType type, string path, string method);

    /// <summary>
    /// Adds a problem to the database
    /// </summary>
    /// <param name="problem">Problem object to add</param>
    Task AddProblemAsync(Problem problem);

    /// <summary>
    /// Increases occurrence for a specified problem
    /// </summary>
    /// <param name="problem">Problem for which you want to increase occurrence</param>
    /// <param name="log">Log with updated problem information</param>
    void UpdateProblem(Problem problem, Log log);

    /// <summary>
    /// Checks if any changes are saved to the database
    /// </summary>
    /// <returns>True if changes are saved, false if not</returns>
    Task<bool> IsSavedAsync();
}
