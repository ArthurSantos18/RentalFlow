using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.UseCases.Commands.Property;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

namespace RentalFlow.Tests.Application.UseCases.Commands.Property;

public sealed class UpdatePropertyCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly UpdatePropertyCommandHandler _handler;

    public UpdatePropertyCommandHandlerTests()
    {
        _handler = new UpdatePropertyCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenExists()
    {
        var propertyId = _fixture.Create<Guid>();
        var newAddress = TestFixtures.MakeAddress(street: "New Street");

        var request = _fixture.Build<UpdatePropertyRequest>()
            .With(r => r.Address, newAddress)
            .With(r => r.RentPrice, 2500m)
            .With(r => r.Bedrooms, 4)
            .With(r => r.IsAvailable, true)
            .With(r => r.IsActive, true)
            .Create();

        var command = _fixture.Build<UpdatePropertyCommand>()
            .With(c => c.Id, propertyId)
            .With(c => c.Request, request)
            .Create();

        var existingProperty = TestFixtures.MakeProperty(id: propertyId, rentPrice: 1000m, bedrooms: 2);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingProperty.Address.Should().Be(newAddress);
        existingProperty.RentPrice.Should().Be(2500m);
        existingProperty.Bedrooms.Should().Be(4);

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenPropertyDoesNotExist()
    {
        var propertyId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdatePropertyRequest>();

        var command = _fixture.Build<UpdatePropertyCommand>()
            .With(c => c.Id, propertyId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}