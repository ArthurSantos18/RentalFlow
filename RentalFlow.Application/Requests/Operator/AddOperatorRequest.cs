namespace RentalFlow.Application.Requests.Operator;

public sealed record AddOperatorRequest
{
    [Description("O nome do operador.")]
    public string Name { get; init; } = string.Empty;

    [Description("O email do operador.")]
    public string Email { get; init; } = string.Empty;

    [Description("A função do operador.")]
    public OperatorRole Role { get; init; }

    [Description("O ID da equipe à qual o operador pertence.")]
    public Guid TeamId { get; init; }
}