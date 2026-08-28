using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public record GetPropertyByIdQuery(Guid Id) : IQuery<Result<GetPropertyResponse>>;
