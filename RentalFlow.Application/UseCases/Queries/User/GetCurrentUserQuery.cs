namespace RentalFlow.Application.UseCases.Queries.User;

public sealed record GetCurrentUserQuery : IQuery<Result<GetCurrentUserResponse>>;
