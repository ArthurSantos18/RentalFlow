namespace RentalFlow.Application.Requests.User;

public record GetUserTokenRequest
{
    public GetUserTokenRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<Guid>? UserIds { get; set; }
    public IEnumerable<string>? RefreshTokens { get; set; }
    public bool? IsRevoked { get; set; }
    public bool? IsExpired { get; set; }
    public DateTime? MinCreatedAt { get; set; }
    public DateTime? MaxCreatedAt { get; set; }
}
