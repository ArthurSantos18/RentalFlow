using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class PropertyMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var address = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
        );

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, _fixture.Create<decimal>())
            .With(r => r.Bedrooms, _fixture.Create<int>())
            .With(r => r.IsAvailable, _fixture.Create<bool>())
            .Create();

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Address.Should().Be(address);
        entity.RentPrice.Should().Be(request.RentPrice);
        entity.Bedrooms.Should().Be(request.Bedrooms);
        entity.IsAvailable.Should().Be(request.IsAvailable);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void ToUpdateDomain_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var address = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
        );

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, _fixture.Create<decimal>())
            .With(r => r.Bedrooms, _fixture.Create<int>())
            .With(r => r.IsAvailable, _fixture.Create<bool>())
            .Create();

        // Act
        var update = request.ToUpdateDomain();

        // Assert
        update.Should().NotBeNull();
        update.Address.Should().Be(request.Address);
        update.RentPrice.Should().Be(request.RentPrice);
        update.Bedrooms.Should().Be(request.Bedrooms);
        update.IsAvailable.Should().Be(request.IsAvailable);
    }

    [Fact]
    public void ToUpdateDomain_ShouldHandleNullValues()
    {
        // Arrange
        var request = new UpdatePropertyRequest();

        // Act
        var update = request.ToUpdateDomain();

        // Assert
        update.Should().NotBeNull();
        update.Address.Should().BeNull();
        update.RentPrice.Should().BeNull();
        update.Bedrooms.Should().BeNull();
        update.IsAvailable.Should().BeNull();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var address = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
        );

        var entity = new PropertyBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithAddress(address)
            .WithRentPrice(_fixture.Create<decimal>())
            .WithBedrooms(_fixture.Create<int>())
            .WithIsAvailable(_fixture.Create<bool>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
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
        var address1 = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
        );
        var address2 = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
        );

        var entity1 = new PropertyBuilder()
            .WithId(Guid.NewGuid())
            .WithAddress(address1)
            .WithRentPrice(_fixture.Create<decimal>())
            .WithBedrooms(_fixture.Create<int>())
            .WithIsAvailable(_fixture.Create<bool>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var entity2 = new PropertyBuilder()
            .WithId(Guid.NewGuid())
            .WithAddress(address2)
            .WithRentPrice(_fixture.Create<decimal>())
            .WithBedrooms(_fixture.Create<int>())
            .WithIsAvailable(_fixture.Create<bool>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var entities = new List<PropertyEntity> { entity1, entity2 };
        var pagedResult = new PagedResult<PropertyEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        // Act
        var response = pagedResult.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(entities.Count);
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}