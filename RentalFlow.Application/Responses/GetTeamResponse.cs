namespace RentalFlow.Application.Responses;

public sealed record GetTeamResponse : BaseResponse
{
    [Description("O nome da equipe.")]
    public string Name { get; init; } = string.Empty;

    [Description("A descrição da equipe.")]
    public string Description { get; init; } = string.Empty;
}