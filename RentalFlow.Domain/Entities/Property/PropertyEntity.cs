using RentalFlow.Domain.Entities.RentalApplication;

namespace RentalFlow.Domain.Entities.Property;

public class PropertyEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Address { get; private set; } = string.Empty;
    public decimal RentPrice { get; private set; }
    public int Bedrooms { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public PropertyEntity(Guid id, string address, decimal rentPrice, int bedrooms, bool isAvailable, List<RentalApplicationEntity> applications)
    {
        Id = id;
        Address = address;
        RentPrice = rentPrice;
        Bedrooms = bedrooms;
        IsAvailable = isAvailable;
        Applications = applications ?? [];
    }

    public static PropertyEntity Empty { get; } = new PropertyEntity
    {
        Id = Guid.NewGuid(),
        Address = string.Empty,
        RentPrice = 0.0m,
        Bedrooms = 0,
        IsAvailable = true,
        Applications = []
    };

    private PropertyEntity() { }

    public PropertyEntity SetId(Guid id) 
    { 
        Id = id;
        return this;
    }

    public PropertyEntity SetAddress(string address) 
    { 
        Address = address; 
        return this;
    }

    public PropertyEntity SetRentPrice(decimal rentPrice) 
    { 
        RentPrice = rentPrice;
        return this;
    }

    public PropertyEntity SetBedrooms(int bedrooms) 
    { 
        Bedrooms = bedrooms;
        return this;
    }

    public PropertyEntity SetIsAvailable(bool isAvailable) 
    { 
        IsAvailable = isAvailable;
        return this;
    }

    public PropertyEntity SetApplications(List<RentalApplicationEntity> applications)
    {
        Applications = applications ?? [];
        return this;
    }

    public PropertyEntity AddApplication(RentalApplicationEntity application)
    {
        Applications.Add(application);
        return this;
    }
    public PropertyEntity RemoveApplication(RentalApplicationEntity application)
    {
        Applications.Remove(application);
        return this;
    }

    public PropertyBuilder ToBuilder() => new()
    {
        Id = Id,
        Address = Address,
        RentPrice = RentPrice,
        Bedrooms = Bedrooms,
        IsAvailable = IsAvailable,
        Applications = Applications
    };
}