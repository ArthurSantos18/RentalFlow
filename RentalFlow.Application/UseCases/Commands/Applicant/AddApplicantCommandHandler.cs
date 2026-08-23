using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Mappers;
using RentalFlow.Domain.Errors;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public sealed class AddApplicantCommandHandler(IApplicantRepository _applicantRepository) : ICommandHandler<AddApplicantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(AddApplicantCommand command, CancellationToken cancellationToken)
    {
        var applicant = await _applicantRepository.GetByCpfAsync(command.Request.Cpf, cancellationToken);

        if (applicant is not null)
        {
            return Result<Guid>.Failure(ApplicantErrors.ApplicantDoesExist);
        }

        var newApplicant = command.Request.ToEntity();

        await _applicantRepository.AddAsync(newApplicant, cancellationToken);

        await _applicantRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(newApplicant.Id);
    }
}
