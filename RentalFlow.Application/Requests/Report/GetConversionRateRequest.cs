namespace RentalFlow.Application.Requests.Report;

public sealed record GetConversionRateRequest
{
    [Description("Data de início para filtrar as propriedades.")]
    public DateTime From { get; init; }

    [Description("Data de término para filtrar as propriedades.")]
    public DateTime To { get; init; }

    [Description("Indica se a taxa de conversão deve ser comparada com a do período anterior.")]
    public bool CompareWithPrevious { get; init; } = false;
}