namespace RentalFlow.Application.Requests.Property;

public sealed record GetPropertyRequest
{
    public GetPropertyRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    [Description("The list of property IDs to filter by.")]
    public IEnumerable<Guid>? Ids { get; set; }

    [Description("The list of cities to filter by.")]
    public IEnumerable<string>? Cities { get; set; }

    [Description("The list of states to filter by.")]
    public IEnumerable<string>? States { get; set; }

    [Description("The list of neighborhoods to filter by.")]
    public IEnumerable<string>? Neighborhoods { get; set; }

    [Description("The list of zip codes to filter by.")]
    public IEnumerable<string>? ZipCodes { get; set; }

    [Description("The minimum rent price to filter by.")]
    public decimal? MinRentPrice { get; set; }

    [Description("The maximum rent price to filter by.")]
    public decimal? MaxRentPrice { get; set; }

    [Description("The minimum number of bedrooms to filter by.")]
    public int? MinBedrooms { get; set; }

    [Description("The maximum number of bedrooms to filter by.")]
    public int? MaxBedrooms { get; set; }

    [Description("Indicates if the property is available.")]
    public bool? IsAvailable { get; set; }

    [Description("Indicates if the property is active.")]
    public bool? IsActive { get; set; }

    [Description("Indicates if the property has applications.")]
    public bool? HasApplications { get; set; }

    [Description("The list of application IDs to filter by.")]
    public IEnumerable<Guid>? ApplicationIds { get; set; }
}