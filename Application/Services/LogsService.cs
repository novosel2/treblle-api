using Domain.Enums;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using System.Diagnostics;
using Application.Exceptions;

namespace Application.Services;

public class LogsService : ILogsService
{
    private readonly ILogsRepository _logsRepository;

    public LogsService(ILogsRepository logsRepository)
    {
        _logsRepository = logsRepository;
    }


    public async Task<List<Log>> GetLogsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLessThan, double? responseMoreThan,
            DateTime? createdFrom, DateTime? createdTo, string? search)
    {
        var logs = await _logsRepository.GetLogsAsync(page, limit, sortBy, sortDir, methods,
                statusCodes, responseLessThan, responseMoreThan, createdFrom, createdTo, search);
        return logs;
    }


    public async Task<List<Problem>> GetProblemsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLessThan, double? responseMoreThan,
            DateTime? createdFrom, DateTime? createdTo)
    {
        var problems = await _logsRepository.GetProblemsAsync(page, limit, sortBy, sortDir, methods,
                statusCodes, responseLessThan, responseMoreThan, createdFrom, createdTo);
        return problems;
    }


    public async Task<Log> AddLogAsync(HttpRequestMessage req, HttpResponseMessage res, Stopwatch sw)
    {
        var log = new Log()
        {
            Method = req.Method.Method,
            Path = req.RequestUri?.ToString() ?? "undefined path",
            ResponseTime = sw.ElapsedMilliseconds,
            StatusCode = (int)res.StatusCode
        };

        log = await _logsRepository.AddLogAsync(log);

        await CheckForProblemsAsync(log);

        if (!await _logsRepository.IsSavedAsync())
            throw new SavingChangesFailedException("Failed while adding the log to database");

        return log;
    }


    private async Task CheckForProblemsAsync(Log log)
    {
        if (log.ResponseTime >= 500)
            await CreateProblemAsync(ProblemType.HighResponseTime, log);

        if (log.StatusCode.ToString()[0] == '5')
            await CreateProblemAsync(ProblemType.ServerError, log);

        if (log.StatusCode == 0)
            await CreateProblemAsync(ProblemType.Timeout, log);

        if (log.StatusCode == 429)
            await CreateProblemAsync(ProblemType.RateLimitExceeded, log);

        if (log.StatusCode == 503)
            await CreateProblemAsync(ProblemType.ServiceUnavailable, log);
    }

    private async Task CreateProblemAsync(ProblemType type, Log log)
    {
        Problem? problem = await _logsRepository.GetExistingProblemAsync(ProblemType.HighResponseTime, log.Path, log.Method);

        if (problem == null)
        {
            Console.WriteLine("CREATING");
            problem = new Problem()
            {
                Method = log.Method,
                Type = type,
                Path = log.Path,
                LastSeen = DateTime.UtcNow,
                LastResponseTime = log.ResponseTime,
                StatusCode = log.StatusCode
            };

            await _logsRepository.AddProblemAsync(problem);
            Console.WriteLine("CREATED");
        }
        else
        {
            _logsRepository.UpdateProblem(problem, log);
        }
    }
}
