namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record TeamStats
{
    [Description("Quantidade total de equipes.")]
    public int Total { get; init; }

    [Description("Quantidade de equipes ativas.")]
    public int Active { get; init; }

    [Description("Quantidade de equipes inativas.")]
    public int Inactive { get; init; }
}
