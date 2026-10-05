namespace RentalFlow.Application.Requests.User;

public sealed record GetUserRequest
{
    public GetUserRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs de usuários para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de emails para filtrar.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("A lista de IDs de operadores para filtrar.")]
    public IEnumerable<Guid>? OperatorIds { get; set; }

    [Description("Indica se o usuário está ativo.")]
    public bool? IsActive { get; set; }

    [Description("Indica se o usuário deve alterar sua senha.")]
    public bool? MustChangePassword { get; set; }
}