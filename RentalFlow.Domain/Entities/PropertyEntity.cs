namespace RentalFlow.Domain.Entities;

public sealed class PropertyEntity : BaseEntity<PropertyEntity>
{
    public Address Address { get; private set; } = Address.Empty;
    public decimal RentPrice { get; private set; }
    public int Bedrooms { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public List<RentalApplicationEntity> Applications { get; private set; } = [];

    public PropertyEntity(
        Address address,
        decimal rentPrice,
        int bedrooms,
        bool isAvailable)
    {
        Address = address;
        RentPrice = rentPrice;
        Bedrooms = bedrooms;
        IsAvailable = isAvailable;
    }

    private PropertyEntity() { }

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
}