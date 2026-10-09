namespace RentalFlow.UnitTests.Application.Mappers;

public sealed class PropertyMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Create<AddPropertyRequest>();

        var entity = request.ToEntity();

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Address.Should().Be(request.Address);
        entity.RentPrice.Should().Be(request.RentPrice);
        entity.Bedrooms.Should().Be(request.Bedrooms);
        entity.IsAvailable.Should().Be(request.IsAvailable);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = _testsFixtures.MakeProperty();
        var request = _fixture.Create<UpdatePropertyRequest>();

        entity.UpdateFrom(request);

        entity.Address.Should().Be(request.Address);
        entity.RentPrice.Should().Be(request.RentPrice);
        entity.Bedrooms.Should().Be(request.Bedrooms);
        entity.IsAvailable.Should().Be(request.IsAvailable!.Value);
        entity.IsActive.Should().Be(request.IsActive!.Value);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = _testsFixtures.MakeProperty();

        var request = _fixture.Build<UpdatePropertyRequest>()
            .Without(r => r.Address)
            .Without(r => r.RentPrice)
            .Without(r => r.Bedrooms)
            .Without(r => r.IsAvailable)
            .Without(r => r.IsActive)
            .Create();

        entity.UpdateFrom(request);

        entity.Address.Should().Be(entity.Address);
        entity.RentPrice.Should().Be(entity.RentPrice);
        entity.Bedrooms.Should().Be(entity.Bedrooms);
        entity.IsAvailable.Should().Be(entity.IsAvailable);
        entity.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = _testsFixtures.MakeProperty();

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Address.Should().Be(entity.Address);
        response.RentPrice.Should().Be(entity.RentPrice);
        response.Bedrooms.Should().Be(entity.Bedrooms);
        response.IsAvailable.Should().Be(entity.IsAvailable);
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<PropertyEntity>
        {
            _testsFixtures.MakeProperty(),
            _testsFixtures.MakeProperty()
        };

        var pagedResult = new PagedResult<PropertyEntity>(
            entities,
            totalResults: 10,
            page: 2,
            pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}