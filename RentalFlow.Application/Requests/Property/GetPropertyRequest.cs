namespace RentalFlow.Application.Requests.Property;

public sealed record GetPropertyRequest
{
    public GetPropertyRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("A lista de IDs de propriedades para filtrar.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("A lista de cidades para filtrar.")]
    public IEnumerable<string>? Cities { get; set; }

    [Description("A lista de estados para filtrar.")]
    public IEnumerable<string>? States { get; set; }

    [Description("A lista de bairros para filtrar.")]
    public IEnumerable<string>? Neighborhoods { get; set; }

    [Description("A lista de códigos postais para filtrar.")]
    public IEnumerable<string>? ZipCodes { get; set; }

    [Description("O preço mínimo do aluguel para filtrar.")]
    public decimal? MinRentPrice { get; set; }

    [Description("O preço máximo do aluguel para filtrar.")]
    public decimal? MaxRentPrice { get; set; }

    [Description("O número mínimo de quartos para filtrar.")]
    public int? MinBedrooms { get; set; }

    [Description("O número máximo de quartos para filtrar.")]
    public int? MaxBedrooms { get; set; }

    [Description("Indica se a propriedade está disponível.")]
    public bool? IsAvailable { get; set; }

    [Description("Indica se a propriedade está ativa.")]
    public bool? IsActive { get; set; }

    [Description("Indica se a propriedade possui aplicações.")]
    public bool? HasApplications { get; set; }

    [Description("A lista de IDs das aplicações para filtrar.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }
}