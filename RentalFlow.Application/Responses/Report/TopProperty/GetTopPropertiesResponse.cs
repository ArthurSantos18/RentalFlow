namespace RentalFlow.Application.Responses.Report.TopProperty;

public sealed record GetTopPropertiesResponse
{
    [Description("Data inicial do período (UTC).")]
    public DateTime? From { get; init; }

    [Description("Data final do período (UTC).")]
    public DateTime? To { get; init; }

    [Description("Limite máximo de itens retornados.")]
    public int Limit { get; init; }

    [Description("Lista de imóveis ordenados pelo número de propostas recebidas.")]
    public IReadOnlyList<TopPropertyItem> Items { get; init; } = [];
}
