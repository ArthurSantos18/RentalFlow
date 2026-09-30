namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class UpdateApplicantCommandHandler(
    IApplicantRepository _applicantRepository,
    ILogger<UpdateApplicantCommandHandler> _logger
    ) : ICommandHandler<UpdateApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateApplicantCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating applicant {ApplicantId}",
            command.Id);

        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            _logger.LogWarning(
                "Applicant {ApplicantId} not found for update: {ErrorCode} {ErrorMessage}",
                command.Id,
                ApplicantErrors.ApplicantNotFound.Code,
                ApplicantErrors.ApplicantNotFound.Message);

            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        applicant.UpdateFrom(command.Request);

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Applicant {ApplicantId} ({FullName}) updated successfully",
            applicant.Id,
            applicant.FullName);

        return Result.Success();
    }
}