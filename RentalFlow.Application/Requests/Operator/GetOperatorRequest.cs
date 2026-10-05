namespace RentalFlow.Application.Requests.Operator;

public sealed class GetOperatorRequest
{
    public GetOperatorRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of operator IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of operator names to filter by.")]
    public IEnumerable<string>? Names { get; set; }

    [Description("The list of operator emails to filter by.")]
    public IEnumerable<string>? Emails { get; set; }

    [Description("The role of the operator to filter by.")]
    public OperatorRole? Role { get; set; }

    [Description("Indicates if the operator is active.")]
    public bool? IsActive { get; set; }

    [Description("Indicates if the operator has applications.")]
    public bool? HasApplications { get; set; }

    [Description("The list of application IDs to filter by.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }

    [Description("The list of team IDs to filter by.")]
    public IEnumerable<Guid>? TeamIds { get; set; }
}