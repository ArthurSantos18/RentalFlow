namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record RentalApplicationStats
{
    [Description("Quantidade total de solicitações de aluguel.")]
    public int Total { get; init; }

    [Description("Quantidade de solicitações agrupadas por status.")]
    public RentalApplicationByStatus ByStatus { get; init; } = new();

    [Description("Valor total financiado pelas solicitações de aluguel.")]
    public decimal TotalFinancedAmount { get; init; }

    [Description("Valor total das solicitações de aluguel.")]
    public decimal TotalAmount { get; init; }

    [Description("Valor médio das solicitações de aluguel.")]
    public decimal AverageTicket { get; init; }
}

public sealed record RentalApplicationByStatus
{
    [Description("Quantidade de solicitações em rascunho.")]
    public int Draft { get; init; }

    [Description("Quantidade de solicitações pendentes.")]
    public int Pending { get; init; }

    [Description("Quantidade de solicitações aprovadas.")]
    public int Approved { get; init; }

    [Description("Quantidade de solicitações rejeitadas.")]
    public int Rejected { get; init; }
}

