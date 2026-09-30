namespace RentalFlow.Application.Services;

public sealed class DataScopeService(ICurrentUserService _currentUserService) : IDataScopeService
{
    public DataScope GetScope()
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Broker) =>
                DataScope.ForOperator(_currentUserService.OperatorId),

            nameof(OperatorRole.Manager) =>
                _currentUserService.TeamId.HasValue ? DataScope.ForTeam(_currentUserService.TeamId.Value) : DataScope.Deny(),

            nameof(OperatorRole.Administrator) =>
                DataScope.Global(),

            _ => DataScope.Deny()
        };
    }
}