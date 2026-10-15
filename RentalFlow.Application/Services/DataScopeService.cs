namespace RentalFlow.Application.Services;

public sealed class DataScopeService(ICurrentUserService _currentUserService) : IDataScopeService
{
    public DataScope GetScope()
    {
        return _currentUserService.Role switch
        {
            nameof(OperatorRole.Broker) =>
                new DataScope.ForOperator(_currentUserService.OperatorId),

            nameof(OperatorRole.Manager) =>
                new DataScope.ForTeam(_currentUserService.TeamId),

            nameof(OperatorRole.Administrator) =>
                new DataScope.Global(),

            _ => new DataScope.Deny()
        };
    }
}