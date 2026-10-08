namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record OperatorStats
{
    [Description("Quantidade total de operadores.")]
    public int Total { get; init; }

    [Description("Quantidade de operadores ativos.")]
    public int Active { get; init; }

    [Description("Quantidade de operadores inativos.")]
    public int Inactive { get; init; }

    [Description("Quantidade de operadores agrupados por função.")]
    public OperatorByRole ByRole { get; init; } = new();
}

public sealed record OperatorByRole
{
    [Description("Quantidade de operadores com a função de corretor.")]
    public int Broker { get; init; }

    [Description("Quantidade de operadores com a função de gerente.")]
    public int Manager { get; init; }

    [Description("Quantidade de operadores com a função de administrador.")]
    public int Administrator { get; init; }
}
