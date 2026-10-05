namespace RentalFlow.Application.Requests.User;

public sealed record GetUserTokenRequest
{
    public GetUserTokenRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of user token IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of user IDs to filter by.")]
    public IEnumerable<Guid>? UserIds { get; set; }

    [Description("The list of refresh tokens to filter by.")]
    public IEnumerable<string>? RefreshTokens { get; set; }

    [Description("Indicates if the token is revoked.")]
    public bool? IsRevoked { get; set; }

    [Description("Indicates if the token is expired.")]
    public bool? IsExpired { get; set; }

    [Description("The minimum creation date to filter by.")]
    public DateTime? MinCreatedAt { get; set; }

    [Description("The maximum creation date to filter by.")]
    public DateTime? MaxCreatedAt { get; set; }
}