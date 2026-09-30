namespace RentalFlow.Application.UseCases.Queries.Operator;

public sealed class GetOperatorByIdQueryHandler(
    IOperatorRepository _repository,
    IRentalApplicationRepository _rentalApplicationRepository,
    ICurrentUserService _currentUserService,
    ILogger<GetOperatorByIdQueryHandler> _logger
    ) : IQueryHandler<GetOperatorByIdQuery, Result<GetOperatorByIdResponse>>
{
    public async Task<Result<GetOperatorByIdResponse>> HandleAsync(GetOperatorByIdQuery query, CancellationToken cancellationToken)
    {
        var @operator = await _repository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        if (@operator is null)
        {
            _logger.LogWarning(
                "Operator {OperatorId} not found: {ErrorCode} {ErrorMessage}",
                query.Id,
                OperatorErrors.OperatorNotFound.Code,
                OperatorErrors.OperatorNotFound.Message);

            return Result<GetOperatorByIdResponse>.Failure(OperatorErrors.OperatorNotFound);
        }

        if (!CanAccess(@operator))
        {
            _logger.LogWarning(
                "Forbidden access to operator {OperatorId} ({OperatorEmail}) by role {Role}: {ErrorCode} {ErrorMessage}",
                @operator.Id,
                @operator.User?.Email,
                _currentUserService.Role,
                UserErrors.Forbidden.Code,
                UserErrors.Forbidden.Message);

            return Result<GetOperatorByIdResponse>.Failure(UserErrors.Forbidden);
        }

        var rentalApplicationsCount = await _rentalApplicationRepository.CountByOperatorAsync(@operator.Id, cancellationToken);

        _logger.LogDebug(
            "Operator {OperatorId} retrieved with {RentalApplicationsCount} rental applications",
            @operator.Id,
            rentalApplicationsCount);

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