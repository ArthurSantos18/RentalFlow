namespace RentalFlow.Application.Requests;

public class PageFilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 60;
}