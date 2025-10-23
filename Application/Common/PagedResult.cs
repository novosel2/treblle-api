namespace Application.Common;

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Limit, int Total)
{
    public bool HasNext => (Page + 1L) * Limit < Total;
}
