namespace RentalFlow.Application.Requests;

public class PageFilterRequest
{
    [Description("O número da página a ser recuperada.")]
    public int Page { get; set; } = 1;

    [Description("O número de itens por página.")]
    public int PageSize { get; set; } = 60;
}