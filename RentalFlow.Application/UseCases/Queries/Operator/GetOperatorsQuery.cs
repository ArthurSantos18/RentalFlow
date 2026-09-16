namespace RentalFlow.Application.UseCases.Queries.Operator;

public sealed record GetOperatorsQuery(GetOperatorRequest Request) : IQuery<Result<PagedResult<GetOperatorResponse>>>;
