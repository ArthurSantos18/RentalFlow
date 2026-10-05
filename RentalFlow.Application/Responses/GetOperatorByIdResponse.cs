namespace RentalFlow.Application.Responses;

public sealed record GetOperatorByIdResponse : BaseResponse
{
    [Description("O nome do operador.")]
    public string Name { get; init; } = string.Empty;

    [Description("A função do operador.")]
    public OperatorRole Role { get; init; }

    [Description("O identificador único da equipe à qual o operador pertence.")]
    public Guid TeamId { get; init; }

    [Description("O nome da equipe à qual o operador pertence.")]
    public string TeamName { get; init; } = string.Empty;

    [Description("O endereço de email do operador.")]
    public string Email { get; init; } = string.Empty;

    [Description("Indica se o operador deve alterar sua senha.")]
    public bool MustChangePassword { get; init; }

    [Description("O identificador único da conta de usuário associada ao operador.")]
    public Guid UserId { get; init; }

    [Description("O número de solicitações associadas ao operador.")]
    public int ApplicationsCount { get; init; }
}