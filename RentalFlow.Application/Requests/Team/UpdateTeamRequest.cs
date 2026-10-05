namespace RentalFlow.Application.Requests.Team;

public sealed record UpdateTeamRequest
{
    [Description("O novo nome para a equipe.")]
    public string? Name { get; init; }

    [Description("A nova descrição para a equipe.")]
    public string? Description { get; init; }

    [Description("O novo status ativo para a equipe.")]
    public bool? IsActive { get; init; }
}