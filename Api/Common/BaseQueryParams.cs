using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common;

public class BaseQueryParams
{
    [Range(0, int.MaxValue)]
    public int Page { get; init; } = 0;

    [Range(1, 200)]
    public int Limit { get; init; } = 20;

    public SortByEnum SortBy { get; init; } = SortByEnum.CreatedAt;
    public SortDirEnum SortDir { get; init; } = SortDirEnum.Desc;

    public MethodsEnum[]? Method { get; init; }

    [Range(100, 599)]
    public int[]? StatusCode { get; init; }

    [Range(0, int.MaxValue)]
    [FromQuery(Name = "responseTime[gte]")]
    public double? ResponseGte { get; init; }

    [Range(0, int.MaxValue)]
    [FromQuery(Name = "responseTime[lte]")]
    public double? ResponseLte { get; init; }

    [FromQuery(Name = "createdAt[gte]")]
    public DateTimeOffset? CreatedFrom { get; init; }

    [FromQuery(Name = "createdAt[lte]")]
    public DateTimeOffset? CreatedTo { get; init; }
}
