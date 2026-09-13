using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class DeleteApplicantCommandHandler(IApplicantRepository _applicantRepository) : ICommandHandler<DeleteApplicantCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteApplicantCommand command, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByIdAsync(command.Id, cancellationToken);

        if (applicant is null)
        {
            return Result.Failure(ApplicantErrors.ApplicantNotFound);
        }

        applicant.MarkAsDeleted();

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}