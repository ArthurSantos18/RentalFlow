using RentalFlow.Application.UseCases.Queries.Operator;

public sealed class GetOperatorByIdQueryHandler(
    IOperatorRepository _repository,
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService
    ) : IQueryHandler<GetOperatorByIdQuery, Result<GetOperatorByIdResponse>>
{
    public async Task<Result<GetOperatorByIdResponse>> HandleAsync(GetOperatorByIdQuery query, CancellationToken cancellationToken)
    {
        var @operator = await _repository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        if (@operator is null)
        {
            return Result<GetOperatorByIdResponse>.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!CanAccess(@operator))
        {
            return Result<GetOperatorByIdResponse>.Failure(UserErrors.Forbidden);
        }

        var rentalApplicationsCount = await _rentalApplicationRepository.CountByOperatorAsync(@operator.Id, cancellationToken);

        return Result<GetOperatorByIdResponse>.Success(@operator.ToDetailedResponse(rentalApplicationsCount));
    }

    private bool CanAccess(OperatorEntity @operator)
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Administrator) => true,

            nameof(OperatorRole.Manager) => @operator.TeamId == _currentUserService.TeamId,

            nameof(OperatorRole.Broker) => @operator.Id == _currentUserService.OperatorId,

            _ => false
        };
    }
}