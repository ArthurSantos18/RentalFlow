using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Domain.Entities.Property;

public class PropertyEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Address Address { get; private set; } = Address.Empty;
    public decimal RentPrice { get; private set; }
    public int Bedrooms { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsActive { get; private set; }
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public PropertyEntity(
        Guid id,
        Address address,
        decimal rentPrice,
        int bedrooms,
        bool isAvailable,
        bool isActive,
        List<RentalApplicationEntity> applications)
    {
        Id = id;
        Address = address;
        RentPrice = rentPrice;
        Bedrooms = bedrooms;
        IsAvailable = isAvailable;
        IsActive = isActive;
        Applications = applications ?? [];
    }

    public static PropertyEntity Empty { get; } = new PropertyEntity
    {
        Id = Guid.NewGuid(),
        Address = Address.Empty,
        RentPrice = 0.0m,
        Bedrooms = 0,
        IsAvailable = true,
        IsActive = false,
        Applications = []
    };

    private PropertyEntity() { }

    public PropertyEntity SetId(Guid id) 
    { 
        Id = id;
        return this;
    }

    public PropertyEntity SetAddress(Address address) 
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

    public PropertyEntity SetIsActive(bool isActive)
    {
        IsActive = isActive;
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

    public void Update(PropertyUpdate update)
    {
        if (update.Address != null)
        {
            SetAddress(update.Address);
        }

        if (update.RentPrice.HasValue)
        {
            SetRentPrice(update.RentPrice.Value);
        }

        if (update.Bedroom.HasValue)
        {
            SetBedrooms(update.Bedroom.Value);
        }

        if (update.IsAvailable.HasValue)
        {
            SetIsAvailable(update.IsAvailable.Value);
        }

        if (update.IsActive.HasValue)
        {
            SetIsActive(update.IsActive.Value);
        }
    }

    public PropertyBuilder ToBuilder() => new()
    {
        Id = Id,
        Address = Address,
        RentPrice = RentPrice,
        Bedrooms = Bedrooms,
        IsAvailable = IsAvailable,
        IsActive = IsActive,
        Applications = Applications
    };
}