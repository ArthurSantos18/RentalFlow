namespace RentalFlow.Domain.Patterns.PagedResult;

public sealed class PagedResult<T>
{
    [Description("The current page number.")]
    public int Page { get; set; }

    [Description("The number of items per page.")]
    public int PageSize { get; set; }

    [Description("The total number of results.")]
    public int TotalResults { get; set; }

    [Description("The total number of pages.")]
    public int TotalPages => (TotalResults + PageSize - 1) / PageSize;

    [Description("The list of results for the current page.")]
    public IEnumerable<T> Results { get; set; } = [];

    [Description("Indicates if there is a previous page.")]
    public bool HasPrevious => Page > 1;

    [Description("Indicates if there is a next page.")]
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