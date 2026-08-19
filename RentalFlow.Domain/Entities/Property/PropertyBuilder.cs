using RentalFlow.Domain.Entities.RentalApplication;

namespace RentalFlow.Domain.Entities.Property;

public sealed class PropertyBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Address { get; set; } = string.Empty;
    public decimal RentPrice { get; set; }
    public int Bedrooms { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<RentalApplicationEntity> Applications { get; set; } = [];

    public static PropertyBuilder Create() => new();

    public PropertyBuilder WithId(Guid id) 
    { 
        Id = id; 
        return this;
    }

    public PropertyBuilder WithAddress(string address) 
    { 
        Address = address;
        return this;
    }

    public PropertyBuilder WithRentPrice(decimal rentPrice) 
    { 
        RentPrice = rentPrice;
        return this;
    }

    public PropertyBuilder WithBedrooms(int bedrooms) 
    { 
        Bedrooms = bedrooms; 
        return this;
    }

    public PropertyBuilder WithIsAvailable(bool isAvailable) 
    { 
        IsAvailable = isAvailable;
        return this;
    }

    public PropertyBuilder WithApplications(List<RentalApplicationEntity> applications)
    {
        Applications = applications ?? [];
        return this;
    }

    public PropertyBuilder AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }

    public PropertyEntity Build()
    {
        return new PropertyEntity(
            Id,
            Address,
            RentPrice,
            Bedrooms,
            IsAvailable,
            Applications
        );
    }
}