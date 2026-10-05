namespace RentalFlow.Application.Responses;

public sealed record ErrorResponse
{
    [Description("O código de erro associado à resposta.")]
    public int Code { get; init; }

    [Description("A mensagem de erro associada à resposta.")]
    public string Message { get; init; } = string.Empty;
}