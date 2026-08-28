using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public record GetPropertiesQuery() : IQuery<Result<IEnumerable<GetPropertyResponse>>>;
