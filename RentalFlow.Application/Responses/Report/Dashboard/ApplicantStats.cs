namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record ApplicantStats
{
    [Description("Quantidade total de inquilinos.")]
    public int Total { get; init; }

    [Description("Quantidade de inquilinos ativos.")]
    public int Active { get; init; }

    [Description("Quantidade de inquilinos inativos.")]
    public int Inactive { get; init; }
}
