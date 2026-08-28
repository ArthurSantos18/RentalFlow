using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Domain.Entities.Property;
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
            street: "Rua das Flores",
            number: "123",
            complement: "Apto 101",
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            zipCode: "01234567"
        );

        var request = _fixture.Build<AddPropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, 2500.00m)
            .With(r => r.Bedrooms, 2)
            .With(r => r.IsAvailable, true)
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
            street: "Rua Nova",
            number: "456",
            complement: null!,
            neighborhood: "Jardim",
            city: "Rio de Janeiro",
            state: "RJ",
            zipCode: "87654321"
        );

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, address)
            .With(r => r.RentPrice, 3000.00m)
            .With(r => r.Bedrooms, 3)
            .With(r => r.IsAvailable, false)
            .With(r => r.IsActive, true)
            .Create();

        // Act
        var update = request.ToUpdateDomain();

        // Assert
        update.Should().NotBeNull();
        update.Address.Should().Be(request.Address);
        update.RentPrice.Should().Be(request.RentPrice);
        update.Bedrooms.Should().Be(request.Bedrooms);
        update.IsAvailable.Should().Be(request.IsAvailable);
        update.IsActive.Should().Be(request.IsActive);
    }

    [Fact]
    public void ToUpdateDomain_ShouldHandleNullValues()
    {
        // Arrange
        var request = new UpdatePropertyRequest(); // Todos os campos null

        // Act
        var update = request.ToUpdateDomain();

        // Assert
        update.Should().NotBeNull();
        update.Address.Should().BeNull();
        update.RentPrice.Should().BeNull();
        update.Bedrooms.Should().BeNull();
        update.IsAvailable.Should().BeNull();
        update.IsActive.Should().BeNull();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var address = new Address(
            street: "Avenida Principal",
            number: "789",
            complement: "Casa 2",
            neighborhood: "Vila Nova",
            city: "Curitiba",
            state: "PR",
            zipCode: "87654321"
        );

        var entity = new PropertyBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithAddress(address)
            .WithRentPrice(2000.00m)
            .WithBedrooms(3)
            .WithIsAvailable(true)
            .WithIsActive(true)
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
    public void ToResponse_ShouldMapListOfEntitiesToResponses()
    {
        // Arrange
        var address1 = new Address(
            street: "Rua das Flores",
            number: "123",
            complement: null!,
            neighborhood: "Centro",
            city: "São Paulo",
            state: "SP",
            zipCode: "01234567"
        );

        var address2 = new Address(
            street: "Avenida Principal",
            number: "456",
            complement: "Apto 202",
            neighborhood: "Jardim",
            city: "Rio de Janeiro",
            state: "RJ",
            zipCode: "87654321"
        );

        var entities = new List<PropertyEntity>
        {
            new PropertyBuilder()
                .WithId(_fixture.Create<Guid>())
                .WithAddress(address1)
                .WithRentPrice(2500.00m)
                .WithBedrooms(2)
                .WithIsAvailable(true)
                .WithIsActive(true)
                .Build(),

            new PropertyBuilder()
                .WithId(_fixture.Create<Guid>())
                .WithAddress(address2)
                .WithRentPrice(3200.00m)
                .WithBedrooms(3)
                .WithIsAvailable(false)
                .WithIsActive(true)
                .Build()
        };

        // Act
        var responses = entities.ToResponse();

        // Assert
        responses.Should().NotBeNull();
        responses.Should().HaveCount(entities.Count);
    }
}