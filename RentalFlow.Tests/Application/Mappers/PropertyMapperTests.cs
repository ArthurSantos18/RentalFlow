namespace RentalFlow.Tests.Application.Mappers;

public sealed class PropertyMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var address = TestsFixtures.MakeAddress();

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, 1500m)
            .With(r => r.Bedrooms, 3)
            .With(r => r.IsAvailable, true)
            .Create();

        var entity = request.ToEntity();

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Address.Should().Be(address);
        entity.RentPrice.Should().Be(1500m);
        entity.Bedrooms.Should().Be(3);
        entity.IsAvailable.Should().BeTrue();
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = TestsFixtures.MakeProperty(rentPrice: 50m, bedrooms: 1, isAvailable: false);
        var newAddress = TestsFixtures.MakeAddress(street: "New Street");

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, newAddress)
            .With(r => r.RentPrice, 2000m)
            .With(r => r.Bedrooms, 4)
            .With(r => r.IsAvailable, true)
            .With(r => r.IsActive, false)
            .Create();

        entity.UpdateFrom(request);

        entity.Address.Should().Be(newAddress);
        entity.RentPrice.Should().Be(2000m);
        entity.Bedrooms.Should().Be(4);
        entity.IsAvailable.Should().BeTrue();
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var address = TestsFixtures.MakeAddress();
        var entity = TestsFixtures.MakeProperty(address: address, rentPrice: 50m, bedrooms: 1, isAvailable: false);

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, (Address?)null)
            .With(r => r.RentPrice, (decimal?)null)
            .With(r => r.Bedrooms, (int?)null)
            .With(r => r.IsAvailable, (bool?)null)
            .With(r => r.IsActive, (bool?)null)
            .Create();

        entity.UpdateFrom(request);

        entity.Address.Should().Be(address);
        entity.RentPrice.Should().Be(50m);
        entity.Bedrooms.Should().Be(1);
        entity.IsAvailable.Should().BeFalse();
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = TestsFixtures.MakeProperty(rentPrice: 2500m, bedrooms: 3, isAvailable: true);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Address.Should().Be(entity.Address);
        response.RentPrice.Should().Be(2500m);
        response.Bedrooms.Should().Be(3);
        response.IsAvailable.Should().BeTrue();
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<PropertyEntity>
        {
            TestsFixtures.MakeProperty(),
            TestsFixtures.MakeProperty(rentPrice: 3000m, bedrooms: 4)
        };

        var pagedResult = new PagedResult<PropertyEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(2);
        response.PageSize.Should().Be(2);
        response.TotalResults.Should().Be(10);
        response.Results.Should().HaveCount(2);
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}