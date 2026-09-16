namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record AddApplicantCommand(AddApplicantRequest Request) : ICommand<Result<Guid>>;
