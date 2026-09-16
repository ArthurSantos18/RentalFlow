namespace RentalFlow.Application.Responses;

public abstract record BaseResponse
{
    [JsonPropertyOrder(-1)]
    public Guid Id { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
