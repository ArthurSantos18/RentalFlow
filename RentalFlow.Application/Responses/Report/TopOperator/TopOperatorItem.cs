namespace RentalFlow.Application.Responses.Report.TopOperator;

public sealed record TopOperatorItem
{
    [Description("ID do operador.")]
    public Guid OperatorId { get; init; }

    [Description("Nome do operador.")]
    public string Name { get; init; } = string.Empty;

    [Description("Papel do operador (Broker, Manager ou Administrator).")]
    public string Role { get; init; } = string.Empty;

    [Description("ID do time ao qual o operador pertence.")]
    public Guid TeamId { get; init; }

    [Description("Nome do time do operador.")]
    public string TeamName { get; init; } = string.Empty;

    [Description("Indica se o operador está ativo.")]
    public bool IsActive { get; init; }

    [Description("Estatísticas das propostas criadas pelo operador.")]
    public TopOperatorStats Stats { get; init; } = new();
}
