namespace RentalFlow.Application.Models
{
    public sealed record DataScope(IEnumerable<Guid>? OperatorIds = null, IEnumerable<Guid>? TeamIds = null, bool IsGlobal = false)
    {
        public static DataScope Global() => new(IsGlobal: true);

        public static DataScope ForOperator(Guid operatorId) => new(OperatorIds: [operatorId]);

        public static DataScope ForTeam(Guid teamId) => new(TeamIds: [teamId]);

        public static DataScope Deny() => new(OperatorIds: [Guid.Empty], TeamIds: [Guid.Empty]);
    }
}
