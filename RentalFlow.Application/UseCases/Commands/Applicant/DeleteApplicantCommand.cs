namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed record DeleteApplicantCommand(Guid Id) : ICommand<Result>;
