using Domain.Enums;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using System.Diagnostics;
using Application.Exceptions;
using Application.Common;
using Application.Common.Dto;

namespace Application.Services;

public class LogsService : ILogsService
{
    private readonly ILogsRepository _logsRepository;

    public LogsService(ILogsRepository logsRepository)
    {
        _logsRepository = logsRepository;
    }


    public async Task<PagedResult<LogDto>> GetLogsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLte, double? responseGte,
            DateTimeOffset? createdFrom, DateTimeOffset? createdTo, string? search, CancellationToken ct)
    {
        var (logs, total) = await _logsRepository.GetLogsAsync(page, limit, sortBy, sortDir, methods,
                statusCodes, responseLte, responseGte, createdFrom, createdTo, search, ct);

        var logDtos = logs.Select(l => l.ToLogDto()).ToList();

        return new PagedResult<LogDto>(logDtos, page, limit, total);
    }


    public async Task<PagedResult<ProblemDto>> GetProblemsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLte, double? responseGte,
            DateTimeOffset? createdFrom, DateTimeOffset? createdTo, CancellationToken ct)
    {
        var (problems, total) = await _logsRepository.GetProblemsAsync(page, limit, sortBy, sortDir, methods,
                statusCodes, responseLte, responseGte, createdFrom, createdTo, ct);

        var problemDtos = problems.Select(p => p.ToProblemDto()).ToList();

        return new PagedResult<ProblemDto>(problemDtos, page, limit, total);
    }


    public async Task<LogDto> AddLogAsync(HttpRequestMessage req, HttpResponseMessage res, Stopwatch sw, CancellationToken ct)
    {
        var log = new Log()
        {
            Method = req.Method.Method,
            Path = req.RequestUri is null ? "undefined path" : req.RequestUri.PathAndQuery,
            ResponseTime = sw.ElapsedMilliseconds,
            StatusCode = (int)res.StatusCode
        };

        log = await _logsRepository.AddLogAsync(log, ct);


        await CheckForProblemsAsync(log, ct);

        if (!await _logsRepository.IsSavedAsync(ct))
            throw new SavingChangesFailedException("Failed while adding the log to database");

        return log.ToLogDto();
    }


    private async Task CheckForProblemsAsync(Log log, CancellationToken ct)
    {
        if (log.ResponseTime >= 300)
            await CreateProblemAsync(ProblemType.HighResponseTime, log, ct);

        if (log.StatusCode >= 500 && log.StatusCode <= 600)
            await CreateProblemAsync(ProblemType.ServerError, log, ct);

        if (log.StatusCode == 0)
            await CreateProblemAsync(ProblemType.Timeout, log, ct);

        if (log.StatusCode == 429)
            await CreateProblemAsync(ProblemType.RateLimitExceeded, log, ct);

        if (log.StatusCode == 503)
            await CreateProblemAsync(ProblemType.ServiceUnavailable, log, ct);
    }

    private async Task CreateProblemAsync(ProblemType type, Log log, CancellationToken ct)
    {
        Problem? problem = await _logsRepository.GetExistingProblemAsync(type, log.Path, log.Method, ct);

        if (problem == null)
        {
            problem = new Problem()
            {
                Method = log.Method,
                Type = type,
                Path = log.Path,
                LastSeen = DateTimeOffset.UtcNow,
                LastResponseTime = log.ResponseTime,
                StatusCode = log.StatusCode
            };

            await _logsRepository.AddProblemAsync(problem, ct);
        }
        else
        {
            _logsRepository.UpdateProblem(problem, log, ct);
        }
    }
}
