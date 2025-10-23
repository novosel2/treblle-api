using Domain.Enums;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;

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
}
