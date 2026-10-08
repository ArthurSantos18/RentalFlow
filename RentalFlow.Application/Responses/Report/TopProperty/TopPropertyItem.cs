namespace RentalFlow.Application.Responses.Report.TopProperty;

public sealed record TopPropertyItem
{
    [Description("ID do imóvel.")]
    public Guid PropertyId { get; init; }

    [Description("Endereço completo do imóvel.")]
    public string Address { get; init; } = string.Empty;

    [Description("Valor do aluguel.")]
    public decimal RentPrice { get; init; }

    [Description("Número de quartos.")]
    public int Bedrooms { get; init; }

    [Description("Indica se o imóvel está disponível para locação.")]
    public bool IsAvailable { get; init; }

    [Description("Estatísticas das propostas recebidas pelo imóvel.")]
    public TopPropertyStats Stats { get; init; } = new();
}
