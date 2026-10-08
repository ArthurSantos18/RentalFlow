namespace RentalFlow.Application.Responses.Report.TopOperator;

public sealed record GetTopOperatorsResponse
{
    [Description("Data inicial do período (UTC).")]
    public DateTime? From { get; init; }

    [Description("Data final do período (UTC).")]
    public DateTime? To { get; init; }

    [Description("Limite máximo de itens retornados.")]
    public int Limit { get; init; }

    [Description("Lista de operadores ordenados pelo número de propostas criadas.")]
    public IReadOnlyList<TopOperatorItem> Items { get; init; } = [];
}