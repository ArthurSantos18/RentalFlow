namespace RentalFlow.Domain.Patterns.PagedResult;

public sealed class PagedResult<T>
{
    [Description("O número da página atual.")]
    public int Page { get; set; }

    [Description("O número de itens por página.")]
    public int PageSize { get; set; }

    [Description("O número total de resultados.")]
    public int TotalResults { get; set; }

    [Description("O número total de páginas.")]
    public int TotalPages => (TotalResults + PageSize - 1) / PageSize;

    [Description("A lista de resultados para a página atual.")]
    public IEnumerable<T> Results { get; set; } = [];

    [Description("Indica se há uma página anterior.")]
    public bool HasPrevious => Page > 1;

    [Description("Indica se há uma página seguinte.")]
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