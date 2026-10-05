namespace RentalFlow.Application.Requests.Property;

public sealed record UpdatePropertyRequest
{
    [Description("O novo endereço da propriedade.")]
    public Address? Address { get; init; }

    [Description("O novo preço mensal do aluguel da propriedade.")]
    public decimal? RentPrice { get; init; }

    [Description("O novo número de quartos na propriedade.")]
    public int? Bedrooms { get; init; }

    [Description("O novo status de disponibilidade para a propriedade.")]
    public bool? IsAvailable { get; init; }

    [Description("O novo status ativo para a propriedade.")]
    public bool? IsActive { get; init; }
}