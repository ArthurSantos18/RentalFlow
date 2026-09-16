namespace RentalFlow.Application.Requests.User;

public record GetUserRequest
{
    public GetUserRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public IEnumerable<Guid>? OperatorIds { get; set; }
    public bool? IsActive { get; set; }
    public bool? MustChangePassword { get; set; }
}
