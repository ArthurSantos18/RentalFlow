namespace RentalFlow.Application.UseCases.Commands.Auth;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : ICommand<Result>;