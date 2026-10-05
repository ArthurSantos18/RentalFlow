namespace RentalFlow.Application.Requests.Property;

public sealed record AddPropertyRequest
{
    [Description("O endereço da propriedade.")]
    public Address Address { get; init; } = Address.Empty;

    [Description("O preço mensal do aluguel da propriedade.")]
    public decimal RentPrice { get; init; }

    [Description("O número de quartos na propriedade.")]
    public int Bedrooms { get; init; }

    [Description("Indica se a propriedade está disponível para aluguel.")]
    public bool IsAvailable { get; init; }
}