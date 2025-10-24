using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common;

public class ProblemQueryParams
{
    public int Page { get; init; } = 0;
    public int Limit { get; init; } = 20;
    public SortByEnum SortBy { get; init; } = SortByEnum.CreatedAt;
    public SortDirEnum SortDir { get; init; } = SortDirEnum.Desc;

    public MethodsEnum[]? Method { get; init; }
    public int[]? StatusCode { get; init; }

    [FromQuery(Name = "responseTime[gte]")]
    public double? ResponseGte { get; init; }

    [FromQuery(Name = "responseTime[lte]")]
    public double? ResponseLte { get; init; }

    [FromQuery(Name = "createdAt[gte]")]
    public DateTimeOffset? CreatedFrom { get; init; }

    [FromQuery(Name = "createdAt[lte]")]
    public DateTimeOffset? CreatedTo { get; init; }
}
