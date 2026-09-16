namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed record UpdateApplicantCommand(Guid Id, UpdateApplicantRequest Request) : ICommand<Result>;
