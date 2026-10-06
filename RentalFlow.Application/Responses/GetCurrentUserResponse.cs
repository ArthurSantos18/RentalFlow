namespace RentalFlow.Application.Responses;

public sealed record GetCurrentUserResponse : BaseResponse
{
    [Description("O e-mail do usuário.")]
    public string Email { get; init; } = string.Empty;

    [Description("Indica se o usuário deve alterar a senha.")]
    public bool MustChangePassword { get; init; }

    [Description("O identificador do operador associado ao usuário.")]
    public Guid? OperatorId { get; init; }

    [Description("O nome do operador associado ao usuário.")]
    public string? OperatorName { get; init; }

    [Description("O cargo do operador associado ao usuário.")]
    public string? OperatorRole { get; init; }

    [Description("O identificador da equipe associada ao usuário.")]
    public Guid? TeamId { get; init; }

    [Description("O nome da equipe associada ao usuário.")]
    public string? TeamName { get; init; }
}