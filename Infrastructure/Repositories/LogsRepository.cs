using Application.Enums;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LogsRepository : ILogsRepository
{
    private readonly AppDbContext _db;

    public LogsRepository(AppDbContext db)
    {
        _db = db;
    }


    public async Task<Log> AddLogAsync(Log log)
    {
        var entry = await _db.AddAsync(log);
        return entry.Entity;
    }


    public async Task<List<Log>> GetLogsAsync(int page, int limit, SortByEnum sortBy, SortDirEnum sortDir,
            MethodsEnum[]? methods, int[]? statusCodes, double? responseLessThan, double? responseMoreThan,
            DateTime? createdFrom, DateTime? createdTo)
    {
        var query = _db.Logs.AsQueryable();

        // Filter by methods
        if (methods != null)
            query = query.Where(l => methods.Select(m => m.ToString()).Contains(l.Method));

        // Filter by status codes
        if (statusCodes != null)
            query = query.Where(l => statusCodes.Contains(l.StatusCode));

        // Filter by response time
        if (responseLessThan != null)
            query = query.Where(l => l.ResponseTime <= responseLessThan);
        if (responseMoreThan != null)
            query = query.Where(l => l.ResponseTime >= responseMoreThan);

        // Filter by date
        if (createdFrom != null)
            query = query.Where(l => l.CreatedAt >= createdFrom);
        if (createdTo != null)
            query = query.Where(l => l.CreatedAt <= createdTo);

        // Sort by and sort direction
        (string by, bool desc) = (sortBy.ToString().ToLower(), sortDir.ToString().ToLower() == "desc");
        query = (by, desc) switch
        {
            ("responsetime", false) => query.OrderBy(r => r.ResponseTime),
            ("responsetime", true) => query.OrderByDescending(r => r.ResponseTime),
            ("statuscode", false) => query.OrderBy(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("statuscode", true) => query.OrderByDescending(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt) // default
        };

        var logs = await query
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync();

        return logs;
    }


    public async Task<bool> IsSavedAsync()
    {
        int saved = await _db.SaveChangesAsync();
        return saved > 0;
    }
}
