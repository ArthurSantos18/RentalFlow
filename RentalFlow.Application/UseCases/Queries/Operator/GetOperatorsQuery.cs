using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.Operator;

public record GetOperatorsQuery(GetOperatorsRequest Request) : IQuery<Result<PagedResult<GetOperatorResponse>>>;
