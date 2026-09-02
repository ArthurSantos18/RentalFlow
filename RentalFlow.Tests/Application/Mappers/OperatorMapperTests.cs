using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class OperatorMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<AddOperatorRequest>()
            .With(r => r.Role, OperatorRole.Broker)
            .Create();

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(request.Name);
        entity.Email.Should().Be(request.Email);
        entity.Role.Should().Be(request.Role);
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToDomain_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<UpdateOperatorRequest>()
            .With(r => r.Role, OperatorRole.Manager)
            .Create();

        // Act
        var update = request.ToUpdateDomain();

        // Assert
        update.Should().NotBeNull();
        update.Name.Should().Be(request.Name);
        update.Email.Should().Be(request.Email);
        update.Role.Should().Be(request.Role);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var entity = new OperatorBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithName(_fixture.Create<string>())
            .WithEmail(_fixture.Create<string>())
            .WithRole(OperatorRole.Administrator)
            .WithIsActive(true)
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be(entity.Name);
        response.Email.Should().Be(entity.Email);
        response.Role.Should().Be(entity.Role);
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        // Arrange
        var entity1 = new OperatorBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithRole(OperatorRole.Broker)
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var entity2 = new OperatorBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithRole(OperatorRole.Manager)
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var entities = new List<OperatorEntity> { entity1, entity2 };
        var pagedResult = new PagedResult<OperatorEntity>(
            entities,
            totalResults: 10,
            page: 2,
            pageSize: 2);

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
