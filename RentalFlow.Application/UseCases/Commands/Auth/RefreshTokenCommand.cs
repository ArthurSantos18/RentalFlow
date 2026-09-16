namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed record RefreshTokenCommand(RefreshTokenRequest Request) : ICommand<Result<LoginResponse>>;