using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Dto;

public record ProblemDto(
    ProblemType Type,
    string Method,
    string Path,
    int Occurrences,
    double LastResponseTime,
    int StatusCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastSeen
);

public static class ProblemExtension
{
    public static ProblemDto ToProblemDto(this Problem problem)
    {
        return new ProblemDto(
            problem.Type,
            problem.Method,
            problem.Path,
            problem.Occurrences,
            problem.LastResponseTime,
            problem.StatusCode,
            problem.CreatedAt,
            problem.LastSeen
        );
    }
}
