namespace RentalFlow.Application.Requests.Operator;

public sealed record UpdateOperatorRequest
{
    [Description("O novo nome do operador.")]
    public string? Name { get; init; }

    [Description("O novo email do operador.")]
    public string? Email { get; init; }

    [Description("A nova função do operador.")]
    public OperatorRole? Role { get; init; }

    [Description("O novo status ativo para o operador.")]
    public bool? IsActive { get; init; }
}