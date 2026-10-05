namespace RentalFlow.Application.Requests.Operator;

public sealed class GetOperatorRequest
{
    public GetOperatorRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs dos operadores para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de nomes dos operadores para filtrar.")]
    public IEnumerable<string>? Names { get; set; }

    [Description("A lista de emails dos operadores para filtrar.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("A função do operador para filtrar.")]
    public OperatorRole? Role { get; set; }

    [Description("Indica se o operador está ativo.")]
    public bool? IsActive { get; set; }

    [Description("Indica se o operador possui aplicações.")]
    public bool? HasApplications { get; set; }

    [Description("A lista de IDs das aplicações para filtrar.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }

    [Description("A lista de IDs das equipes para filtrar.")]
    public IEnumerable<Guid>? TeamIds { get; set; }
}