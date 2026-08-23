using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.Applicant;

public record AddApplicantCommand(AddApplicantRequest Request) : ICommand<Result<Guid>>;
