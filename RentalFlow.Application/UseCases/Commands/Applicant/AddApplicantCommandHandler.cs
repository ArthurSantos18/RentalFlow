namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class AddApplicantCommandHandler(
    IApplicantRepository _applicantRepository,
    ILogger<AddApplicantCommandHandler> _logger
    ) : ICommandHandler<AddApplicantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddApplicantCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        _logger.LogInformation(
            "Creating applicant {FullName} with CPF ending in {Last4}",
            request.FullName,
            request.Cpf.Length >= 4 ? request.Cpf[^4..] : "****");

        var applicant = await _applicantRepository.GetByCpfAsync(command.Request.Cpf, cancellationToken);

        if (applicant is not null)
        {
            _logger.LogWarning(
                "Applicant with CPF ending in {Last4} already exists (ID: {ApplicantId}): {ErrorCode} {ErrorMessage}",
                request.Cpf.Length >= 4 ? request.Cpf[^4..] : "****",
                applicant.Id,
                ApplicantErrors.ApplicantAlreadyExists.Code,
                ApplicantErrors.ApplicantAlreadyExists.Message);

            return Result<Guid>.Failure(ApplicantErrors.ApplicantAlreadyExists);
        }

        var newApplicant = command.Request.ToEntity();

        await _applicantRepository.AddAsync(newApplicant, cancellationToken);

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Applicant {ApplicantId} ({FullName}) created successfully",
            newApplicant.Id,
            newApplicant.FullName);

        return Result<Guid>.Success(newApplicant.Id);
    }
}