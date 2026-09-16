namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class UpdateApplicantCommandHandler(IApplicantRepository _applicantRepository) : ICommandHandler<UpdateApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateApplicantCommand command, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        applicant.UpdateFrom(command.Request);

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
