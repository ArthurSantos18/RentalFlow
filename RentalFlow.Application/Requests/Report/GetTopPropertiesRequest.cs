namespace RentalFlow.Application.Requests.Report;

public sealed record GetTopPropertiesRequest
{
    [Description("Número máximo de propriedades a serem retornadas.")]
    public int Limit { get; init; } = 10;

    [Description("Data de início para filtrar as propriedades.")]
    public DateTime? From { get; init; }

    [Description("Data de término para filtrar as propriedades.")]
    public DateTime? To { get; init; }
}
