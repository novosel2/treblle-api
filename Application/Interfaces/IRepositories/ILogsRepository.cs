using Domain.Enums;
using Domain.Entities;
using Application.Common.Dto;

namespace Application.Interfaces.IRepositories;

public interface ILogsRepository
{
    /// <summary>
    /// Adds a log in the database
    /// </summary>
    /// <param name="log">Log object to be added</param>
    /// <returns>Added log object</returns>
    Task<Log> AddLogAsync(Log log, CancellationToken ct);

    /// <summary>
    /// Gets specified number of logs for specified page
    /// </summary>
    /// <param name="q">Query params</param>
    /// <returns>List of logs and total number of logs</returns>
    Task<(List<Log>, int Total)> GetLogsAsync(QueryDto q, CancellationToken ct);

    /// <summary>
    /// Gets specified number of problems for specified page
    /// </summary>
    /// <param name="q">Query params</param>
    /// <returns>List of problems and total number of problems</returns>
    Task<(List<Problem>, int Total)> GetProblemsAsync(QueryDto q, CancellationToken ct);

    /// <summary>
    /// Checks if the same problem already occurred in the last 20 minutes
    /// </summary>
    /// <param name="type">Problem type that occurred</param>
    /// <param name="path">Path with the problem occurrence</param>
    /// <param name="method">Method of the request</param>
    /// <returns>True if problem already occurred, false if not</returns>
    Task<Problem?> GetExistingProblemAsync(ProblemType type, string path, string method, CancellationToken ct);

    /// <summary>
    /// Adds a problem to the database
    /// </summary>
    /// <param name="problem">Problem object to add</param>
    Task AddProblemAsync(Problem problem, CancellationToken ct);

    /// <summary>
    /// Increases occurrence for a specified problem
    /// </summary>
    /// <param name="problem">Problem for which you want to increase occurrence</param>
    /// <param name="log">Log with updated problem information</param>
    void UpdateProblem(Problem problem, Log log, CancellationToken ct);

    /// <summary>
    /// Checks if any changes are saved to the database
    /// </summary>
    /// <returns>True if changes are saved, false if not</returns>
    Task<bool> IsSavedAsync(CancellationToken ct);
}
