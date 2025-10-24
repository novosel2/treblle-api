using Domain.Enums;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Common.Dto;

namespace Infrastructure.Repositories;

public class LogsRepository : ILogsRepository
{
    private readonly AppDbContext _db;

    public LogsRepository(AppDbContext db)
    {
        _db = db;
    }


    public async Task<Log> AddLogAsync(Log log, CancellationToken ct)
    {
        var entry = await _db.AddAsync(log);
        return entry.Entity;
    }


    public async Task<(List<Log>, int Total)> GetLogsAsync(QueryDto q, CancellationToken ct)
    {
        var query = _db.Logs.AsNoTracking();

        // Filter by methods
        if (q.Methods != null)
            query = query.Where(l => q.Methods.Select(m => m.ToString()).Contains(l.Method));

        // Filter by status codes
        if (q.StatusCodes != null)
            query = query.Where(l => q.StatusCodes.Contains(l.StatusCode));

        // Filter by response time
        if (q.ResponseLte != null)
            query = query.Where(l => l.ResponseTime <= q.ResponseLte);
        if (q.ResponseGte != null)
            query = query.Where(l => l.ResponseTime >= q.ResponseGte);

        // Filter by date
        if (q.CreatedFrom != null)
            query = query.Where(l => l.CreatedAt >= q.CreatedFrom);
        if (q.CreatedTo != null)
            query = query.Where(l => l.CreatedAt <= q.CreatedTo);

        // Search function
        if (!string.IsNullOrEmpty(q.Search))
            query = SearchFunction(query, q.Search, ct);

        // Sort by and sort direction
        (string by, bool desc) = (q.SortBy.ToString().ToLower(), q.SortDir.ToString().ToLower() == "desc");
        query = (by, desc) switch
        {
            ("responsetime", false) => query.OrderBy(r => r.ResponseTime),
            ("responsetime", true) => query.OrderByDescending(r => r.ResponseTime),
            ("statuscode", false) => query.OrderBy(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("statuscode", true) => query.OrderByDescending(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt) // default
        };

        int total = await query.CountAsync();

        var logs = await query
            .Skip(q.Page * q.Limit)
            .Take(q.Limit)
            .ToListAsync();

        return (logs, total);
    }


    public async Task<(List<Problem>, int Total)> GetProblemsAsync(QueryDto q, CancellationToken ct)
    {
        var query = _db.Problems.AsNoTracking();

        // Filter by methods
        if (q.Methods != null)
            query = query.Where(p => q.Methods.Select(m => m.ToString()).Contains(p.Method));

        // Filter by status codes
        if (q.StatusCodes != null)
            query = query.Where(p => q.StatusCodes.Contains(p.StatusCode));

        // Filter by response time
        if (q.ResponseLte != null)
            query = query.Where(p => p.LastResponseTime <= q.ResponseLte);
        if (q.ResponseGte != null)
            query = query.Where(p => p.LastResponseTime >= q.ResponseGte);

        // Filter by date
        if (q.CreatedFrom != null)
            query = query.Where(p => p.CreatedAt >= q.CreatedFrom);
        if (q.CreatedTo != null)
            query = query.Where(p => p.CreatedAt <= q.CreatedTo);

        // Sort by and sort direction
        (string by, bool desc) = (q.SortBy.ToString().ToLower(), q.SortDir.ToString().ToLower() == "desc");
        query = (by, desc) switch
        {
            ("responsetime", false) => query.OrderBy(r => r.LastResponseTime),
            ("responsetime", true) => query.OrderByDescending(r => r.LastResponseTime),
            ("statuscode", false) => query.OrderBy(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("statuscode", true) => query.OrderByDescending(r => r.StatusCode).ThenByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt) // default
        };

        int total = await query.CountAsync();

        var problems = await query
            .Skip(q.Page * q.Limit)
            .Take(q.Limit)
            .ToListAsync();

        return (problems, total);
    }


    public async Task<Problem?> GetExistingProblemAsync(ProblemType type, string path, string method, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        Problem? problem = await _db.Problems.FirstOrDefaultAsync(
                p => p.Type.ToString() == type.ToString()
                && p.Path == path
                && p.Method == method
                && (now - p.LastSeen) < TimeSpan.FromMinutes(20));

        return problem;
    }


    public async Task AddProblemAsync(Problem problem, CancellationToken ct)
    {
        await _db.Problems.AddAsync(problem);
    }


    public void UpdateProblem(Problem problem, Log log, CancellationToken ct)
    {
        problem.Occurrences++;
        problem.StatusCode = log.StatusCode;
        problem.LastResponseTime = log.ResponseTime;
        problem.LastSeen = DateTime.UtcNow;
        _db.Problems.Entry(problem).State = EntityState.Modified;
    }


    public async Task<bool> IsSavedAsync(CancellationToken ct)
    {
        int saved = await _db.SaveChangesAsync();
        return saved > 0;
    }


    private IQueryable<Log> SearchFunction(IQueryable<Log> query, string search, CancellationToken ct)
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
