using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Domain.Entities.Property;

public sealed class PropertyBuilder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Address Address { get; set; } = Address.Empty;
    public decimal RentPrice { get; set; }
    public int Bedrooms { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsActive { get; set; }
    public List<RentalApplicationEntity> Applications { get; set; } = [];

    public static PropertyBuilder Create() => new();

    public PropertyBuilder WithId(Guid id) 
    { 
        Id = id; 
        return this;
    }

    public PropertyBuilder WithAddress(Address address) 
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

    public PropertyBuilder WithIsActive(bool isActive)
    {
        IsActive = isActive;
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
            IsActive,
            Applications
        );
    }
}