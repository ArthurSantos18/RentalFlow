using LiteBus.Queries.Abstractions;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.Responses;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.Patterns.Result;

namespace RentalFlow.Application.UseCases.Queries.RentalApplication;

public record GetRentalApplicationsQuery(GetRentalApplicationRequest Request) : IQuery<Result<PagedResult<GetRentalApplicationResponse>>>;
