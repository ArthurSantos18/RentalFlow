namespace RentalFlow.Application.UseCases.Queries.Property;

public record GetPropertiesQuery(GetPropertyRequest Request) : IQuery<Result<PagedResult<GetPropertyResponse>>>;
