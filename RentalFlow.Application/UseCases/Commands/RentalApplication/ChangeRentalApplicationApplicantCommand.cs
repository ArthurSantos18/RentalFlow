using LiteBus.Commands.Abstractions;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Commands.RentalApplication;

public record ChangeRentalApplicationApplicantCommand(Guid Id, ChangeRentalApplicationApplicantRequest Request) : ICommand<Result>;
