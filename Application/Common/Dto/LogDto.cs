using Domain.Entities;

namespace Application.Common.Dto;

public record LogDto(
    string Method,
    int StatusCode,
    string Path,
    double ResponseTime,
    DateTimeOffset CreatedAt
);

public static class LogExtension
{
    public static LogDto ToLogDto(this Log log)
    {
        return new LogDto(
            Method: log.Method,
            StatusCode: log.StatusCode,
            Path: log.Path,
            ResponseTime: log.ResponseTime,
            CreatedAt: log.CreatedAt
        );
    }
}
