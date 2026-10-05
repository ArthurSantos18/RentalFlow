namespace RentalFlow.Application.Responses;

public abstract record BaseResponse
{
    [JsonPropertyOrder(-1)]
    [Description("O identificador único da entidade.")]
    public Guid Id { get; init; }

    [Description("Indica se a entidade está ativa.")]
    public bool IsActive { get; init; }

    [Description("A data e hora quando a entidade foi criada.")]
    public DateTime CreatedAt { get; init; }

    [Description("A data e hora quando a entidade foi atualizada pela última vez.")]
    public DateTime? UpdatedAt { get; init; }
}