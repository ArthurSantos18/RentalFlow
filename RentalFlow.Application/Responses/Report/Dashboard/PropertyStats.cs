namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record PropertyStats
{
    [Description("Quantidade total de imóveis.")]
    public int Total { get; init; }

    [Description("Quantidade de imóveis disponíveis.")]
    public int Available { get; init; }

    [Description("Quantidade de imóveis alugados.")]
    public int Rented { get; init; }

    [Description("Quantidade de imóveis ativos.")]
    public int Active { get; init; }

    [Description("Quantidade de imóveis inativos.")]
    public int Inactive { get; init; }
}
