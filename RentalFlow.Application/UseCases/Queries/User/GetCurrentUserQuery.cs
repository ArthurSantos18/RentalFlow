namespace RentalFlow.Application.UseCases.Queries.User;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<Result<GetCurrentUserResponse>>;
