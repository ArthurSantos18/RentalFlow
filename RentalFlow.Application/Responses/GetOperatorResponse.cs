namespace RentalFlow.Application.Responses;

public record GetOperatorResponse : BaseResponse
{
    public string Name { get; init; } = string.Empty;
    public OperatorRole Role { get; init; } 
    public Guid TeamId { get; init; }
    public string TeamName { get; init; } = string.Empty;

}
