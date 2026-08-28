using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record UpdateApplicantCommand(Guid Id, UpdateApplicantRequest Request) : ICommand<Result>;
