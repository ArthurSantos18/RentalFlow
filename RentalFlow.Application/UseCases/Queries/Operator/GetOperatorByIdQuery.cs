namespace RentalFlow.Application.UseCases.Queries.Operator;

public sealed record GetOperatorByIdQuery(Guid Id) : IQuery<Result<GetOperatorByIdResponse>>;