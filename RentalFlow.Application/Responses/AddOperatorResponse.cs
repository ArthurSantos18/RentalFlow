namespace RentalFlow.Application.Responses;

public record AddOperatorResponse
{
    [Description("O identificador único do novo operador.")]
    public Guid Id { get; init; }

    [Description("O nome do novo operador.")]
    public string Name { get; init; } = string.Empty;

    [Description("O email do novo operador.")]
    public string Email { get; init; } = string.Empty;

    [Description("A função do novo operador.")]
    public string Role { get; init; } = string.Empty;

    [Description("A senha temporária para o novo operador.")]
    public string TemporaryPassword { get; init; } = string.Empty;

    [Description("A mensagem associada à resposta.")]
    public string Message { get; init; } = string.Empty;
}