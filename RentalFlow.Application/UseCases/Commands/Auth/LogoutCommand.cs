namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed record LogoutCommand(LogoutRequest Request) : ICommand<Result>;
