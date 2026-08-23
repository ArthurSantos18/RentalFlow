using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class UpdateApplicantCommandHandler(IApplicantRepository _applicantRepository) : ICommandHandler<UpdateApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateApplicantCommand command, CancellationToken cancellationToken = default)
    {
        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        applicant.ApplyUpdates(command.Request);

        _applicantRepository.Update(applicant);

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
