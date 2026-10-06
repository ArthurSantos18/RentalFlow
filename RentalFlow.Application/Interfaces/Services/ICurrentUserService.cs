namespace RentalFlow.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid OperatorId { get; }
    Guid? TeamId { get; }
    string Name { get; }
    string Email { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}