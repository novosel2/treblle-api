using Domain.Enums;
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
            DateTime? createdFrom, DateTime? createdTo, string? search)
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

        // Search function
        if (!string.IsNullOrEmpty(search))
            query = SearchFunction(query, search);

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


    private IQueryable<Log> SearchFunction(IQueryable<Log> query, string search)
    {
        var qLower = search.Trim().ToLowerInvariant();

        // method?
        var methods = new[] { "get", "post", "put", "patch", "delete", "head", "options" };
        bool looksLikeMethod = methods.Contains(qLower);

        // status ranges or exact?
        bool isRange5xx = qLower is "5xx" or "4xx" or "3xx" or "2xx" or "1xx";
        bool isInt = int.TryParse(qLower, out var qCode);

        if (looksLikeMethod)
            query = query.Where(r => r.Method.ToLower() == qLower);
        else if (isRange5xx)
        {
            int prefix = qLower[0] - '0';
            int start = prefix * 100;
            int end = start + 99;
            query = query.Where(r => r.StatusCode >= start && r.StatusCode <= end);
        }
        else if (isInt && qCode is >= 100 and <= 599)
            query = query.Where(r => r.StatusCode == qCode);
        else
        {
            var terms = qLower.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var term in terms)
            {
                query = query.Where(r => EF.Functions.ILike(r.Path, $"%{term}%"));
            }
        }

        return query;
    }
}
