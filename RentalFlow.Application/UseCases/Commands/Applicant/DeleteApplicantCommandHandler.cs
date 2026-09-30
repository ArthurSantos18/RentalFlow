namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class DeleteApplicantCommandHandler(
    IApplicantRepository _applicantRepository,
    IRentalApplicationRepository _rentalApplicationRepository,
    ILogger<DeleteApplicantCommandHandler> _logger
    ) : ICommandHandler<DeleteApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteApplicantCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
             "Deleting applicant {ApplicantId}",
             command.Id);

        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            _logger.LogWarning(
                "Applicant {ApplicantId} not found for deletion: {ErrorCode} {ErrorMessage}",
                command.Id,
                ApplicantErrors.ApplicantNotFound.Code,
                ApplicantErrors.ApplicantNotFound.Message);

            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        var hasApplications = await _rentalApplicationRepository.ApplicantHasApplicationsAsync(applicant.Id, cancellationToken);

        if (hasApplications)
        {
            _logger.LogWarning(
                "Cannot delete applicant {ApplicantId} ({FullName}) because they have associated rental applications: {ErrorCode} {ErrorMessage}",
                applicant.Id,
                applicant.FullName,
                ApplicantErrors.ApplicantHasApplications.Code,
                ApplicantErrors.ApplicantHasApplications.Message);

            return Result.Failure(ApplicantErrors.ApplicantHasApplications);
        }

        applicant.MarkAsDeleted();

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Applicant {ApplicantId} ({FullName}) deleted successfully",
            applicant.Id,
            applicant.FullName);

        return Result.Success();
    }
}