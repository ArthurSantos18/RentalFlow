namespace RentalFlow.Application.UseCases.Queries.Operator;

public record GetOperatorsQuery(GetOperatorRequest Request) : IQuery<Result<PagedResult<GetOperatorResponse>>>;
