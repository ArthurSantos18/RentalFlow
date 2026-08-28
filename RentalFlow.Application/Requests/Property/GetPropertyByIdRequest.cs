namespace RentalFlow.Application.Requests.Property;

public record GetPropertyByIdRequest
{
    public Guid Id { get; init; }
};
