namespace RentalFlow.Application.Responses;

public sealed record GetPropertyResponse : BaseResponse
{
    public Address Address { get; init; } = Address.Empty;

    [Description("O preço de aluguel da propriedade.")]
    public decimal RentPrice { get; init; }

    [Description("O número de quartos na propriedade.")]
    public int Bedrooms { get; init; }

    [Description("Indica se a propriedade está disponível para aluguel.")]
    public bool IsAvailable { get; init; }
}