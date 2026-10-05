namespace RentalFlow.Application.Requests.User;

public sealed record GetUserRequest
{
    public GetUserRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of user IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of emails to filter by.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("The list of operator IDs to filter by.")]
    public IEnumerable<Guid>? OperatorIds { get; set; }

    [Description("Indicates if the user is active.")]
    public bool? IsActive { get; set; }

    [Description("Indicates if the user must change their password.")]
    public bool? MustChangePassword { get; set; }
}