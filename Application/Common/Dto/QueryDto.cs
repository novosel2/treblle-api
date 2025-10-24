using Domain.Enums;

namespace Application.Common.Dto;

public record QueryDto(
    int Page,
    int Limit,
    SortByEnum SortBy,
    SortDirEnum SortDir,
    IReadOnlyList<MethodsEnum>? Methods,
    IReadOnlyList<int>? StatusCodes,
    double? ResponseGte,
    double? ResponseLte,
    DateTimeOffset? CreatedFrom,
    DateTimeOffset? CreatedTo,
    string? Search);
