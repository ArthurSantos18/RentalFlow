using RentalFlow.Domain.Enums;

namespace RentalFlow.Application.Requests.Operator;

public record GetOperatorRequest
{
    public GetOperatorRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public OperatorRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasApplications { get; set; }
    public IEnumerable<Guid>? ApplicationIds { get; set; }
}
