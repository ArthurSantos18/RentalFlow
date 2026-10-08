namespace RentalFlow.Application.Responses.Report.Dashboard;

public sealed record DashboardResponse
{
    [Description("Estatísticas dos inquilinos.")]
    public ApplicantStats Applicants { get; init; } = new();

    [Description("Estatísticas dos imóveis.")]
    public PropertyStats Properties { get; init; } = new();

    [Description("Estatísticas das solicitações de aluguel.")]
    public RentalApplicationStats RentalApplications { get; init; } = new();

    [Description("Estatísticas dos operadores.")]
    public OperatorStats Operators { get; init; } = new();

    [Description("Estatísticas das equipes.")]
    public TeamStats Teams { get; init; } = new();

    [Description("Taxa de conversão das solicitações de aluguel.")]
    public double ConversionRate { get; init; }
}
