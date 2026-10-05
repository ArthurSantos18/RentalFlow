namespace RentalFlow.Application.Responses;

public abstract record BaseResponse
{
    [JsonPropertyOrder(-1)]
    [Description("The unique identifier of the entity.")]
    public Guid Id { get; init; }

    [Description("Indicates whether the entity is active.")]
    public bool IsActive { get; init; }

    [Description("The date and time when the entity was created.")]
    public DateTime CreatedAt { get; init; }

    [Description("The date and time when the entity was last updated.")]
    public DateTime? UpdatedAt { get; init; }
}