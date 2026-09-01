namespace RentalFlow.Domain.Patterns.PagedResult;

public sealed class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalResults { get; set; }
    public int TotalPages => (TotalResults + PageSize - 1) / PageSize;
    public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PagedResult() { }

    public PagedResult(IEnumerable<T> results, int totalResults, int page, int pageSize)
    {
        Results = results;
        TotalResults = totalResults;
        Page = page;
        PageSize = pageSize;
    }
}