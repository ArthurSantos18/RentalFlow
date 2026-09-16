namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed record LoginCommand(LoginRequest Request) : ICommand<Result<LoginResponse>>;
