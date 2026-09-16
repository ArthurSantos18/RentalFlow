namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed record AddApplicantCommand(AddApplicantRequest Request) : ICommand<Result<Guid>>;
