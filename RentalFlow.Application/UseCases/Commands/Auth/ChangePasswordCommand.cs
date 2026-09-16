namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : ICommand<Result<string>>;
