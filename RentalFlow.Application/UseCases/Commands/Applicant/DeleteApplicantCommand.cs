namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record DeleteApplicantCommand(Guid Id) : ICommand<Result>;
