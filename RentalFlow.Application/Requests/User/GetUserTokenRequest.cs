namespace RentalFlow.Application.Requests.User;

public sealed record GetUserTokenRequest
{
    public GetUserTokenRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs de tokens de usuário para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de IDs de usuários para filtrar.")]
    public IEnumerable<Guid>? UserIds { get; set; }

    [Description("A lista de tokens de atualização para filtrar.")]
    public IEnumerable<string>? RefreshTokens { get; set; }

    [Description("Indica se o token está revogado.")]
    public bool? IsRevoked { get; set; }

    [Description("Indica se o token está expirado.")]
    public bool? IsExpired { get; set; }

    [Description("A data mínima de criação para filtrar.")]
    public DateTime? MinCreatedAt { get; set; }

    [Description("A data máxima de criação para filtrar.")]
    public DateTime? MaxCreatedAt { get; set; }
}