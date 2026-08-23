using LiteBus.Commands.Abstractions;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record DeleteApplicantCommand(Guid Id) : ICommand<Result>;
