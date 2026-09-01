namespace RentalFlow.Application.Requests.Property;

public class GetPropertyRequest
{
    public GetPropertyRequest() => PageFilter = new PageFilterRequest { Page = 1, PageSize = 60 };

    public PageFilterRequest PageFilter { get; set; }

    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Cities { get; set; }
    public IEnumerable<string>? States { get; set; }
    public IEnumerable<string>? Neighborhoods { get; set; }
    public IEnumerable<string>? ZipCodes { get; set; }
    public decimal? MinRentPrice { get; set; }
    public decimal? MaxRentPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasApplications { get; set; }
    public IEnumerable<Guid>? ApplicationIds { get; set; }
}