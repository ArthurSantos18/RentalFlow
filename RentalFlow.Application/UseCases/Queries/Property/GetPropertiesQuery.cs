using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Property;

public record GetPropertiesQuery(GetPropertiesRequest Request) : IQuery<Result<PagedResult<GetPropertyResponse>>>;
