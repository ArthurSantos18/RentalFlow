using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Property;
using RentalFlow.Application.UseCases.Queries.Property;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Domain.ValueObject;

namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetPropertiesQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPropertyRepository> _repositoryMock = new();
    private readonly GetPropertiesQueryHandler _handler;

    public GetPropertiesQueryHandlerTests()
    {
        _handler = new GetPropertiesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPropertiesExist()
    {
        // Arrange
        var request = _fixture.Create<GetPropertyRequest>();
        var query = _fixture.Build<GetPropertiesQuery>()
            .With(q => q.Request, request)
            .Create();

        var address = new Address(
            street: _fixture.Create<string>(),
            number: _fixture.Create<string>(),
            complement: _fixture.Create<string>(),
            neighborhood: _fixture.Create<string>(),
            city: _fixture.Create<string>(),
            state: _fixture.Create<string>(),
            zipCode: _fixture.Create<string>()
            );

        var property = new PropertyBuilder()
            .WithId(Guid.NewGuid())
            .WithAddress(address)
            .WithRentPrice(_fixture.Create<decimal>())
            .WithBedrooms(_fixture.Create<int>())
            .WithIsAvailable(_fixture.Create<bool>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var properties = new List<PropertyEntity> { property };
        var pagedResult = new PagedResult<PropertyEntity>(
            results: properties,
            totalResults: 1,
            page: 1,
            pageSize: 60
            );

        _repositoryMock
            .Setup(r => r.GetPropertiesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetPropertiesAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}