namespace RentalFlow.Application.UseCases.Queries.Property;

public sealed record GetPropertiesQuery(GetPropertyRequest Request) : IQuery<Result<PagedResult<GetPropertyResponse>>>;
