namespace RentalFlow.Domain.ValueObject;

public record Address
{
    public string Street { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public string Complement { get; init; } = string.Empty;
    public string Neighborhood { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;

    private Address() { }

    public Address(
        string street,
        string number,
        string complement,
        string neighborhood,
        string city,
        string state,
        string zipCode)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state.ToUpper();
        ZipCode = zipCode.Replace("-", "");
    }

    public static Address Empty { get; } = new Address
    {
        Street = string.Empty,
        Number = string.Empty,
        Complement = string.Empty,
        Neighborhood = string.Empty,
        City = string.Empty,
        State = string.Empty,
        ZipCode = string.Empty
    };

    public string GetFullAddress()
    {
        var complement = string.IsNullOrEmpty(Complement) ? "" : $", {Complement}";
        return $"{Street}, {Number}{complement}, {Neighborhood}, {City} - {State}, {ZipCode}";
    }
}