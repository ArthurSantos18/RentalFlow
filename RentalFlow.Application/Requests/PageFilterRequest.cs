namespace RentalFlow.Application.Requests;

public class PageFilterRequest
{
    [Description("The page number to retrieve.")]
    public int Page { get; set; } = 1;

    [Description("The number of items per page.")]
    public int PageSize { get; set; } = 60;
}