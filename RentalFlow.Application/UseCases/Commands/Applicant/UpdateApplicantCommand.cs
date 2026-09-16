namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record UpdateApplicantCommand(Guid Id, UpdateApplicantRequest Request) : ICommand<Result>;
