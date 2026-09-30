namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class DeleteApplicantCommandHandler(
    IApplicantRepository _applicantRepository,
    IRentalApplicationRepository _rentalApplicationRepository
    ) : ICommandHandler<DeleteApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteApplicantCommand command, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        var hasApplications = await _rentalApplicationRepository.ApplicantHasApplicationsAsync(applicant.Id, cancellationToken);

        if (hasApplications)
        {
            return Result.Failure(ApplicantErrors.ApplicantHasApplications);
        }

        applicant.MarkAsDeleted();

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}