using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Team;
using RentalFlow.Domain.Entities.Team;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class TeamMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<AddTeamRequest>()
            .With(r => r.Name, "Team Alpha")
            .With(r => r.Description, "Description Alpha")
            .Create();

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.Name.Should().Be(request.Name);
        entity.Description.Should().Be(request.Description);
        entity.IsActive.Should().BeTrue();
        entity.Operators.Should().BeEmpty();
    }

    [Fact]
    public void UpdateEntity_ShouldMapAllFieldsCorrectly_WhenExistingProvided()
    {
        // Arrange
        var request = _fixture.Build<UpdateTeamRequest>()
            .With(r => r.Name, "Updated Name")
            .With(r => r.Description, "Updated Description")
            .Create();

        var existing = new TeamBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithName("Old Name")
            .WithDescription("Old Description")
            .WithIsActive(true)
            .Build();

        // Act
        var entity = request.UpdateEntity(existing);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(existing.Id);
        entity.Name.Should().Be(request.Name ?? existing.Name);
        entity.Description.Should().Be(request.Description ?? existing.Description);
        entity.IsActive.Should().Be(existing.IsActive);
    }

    [Fact]
    public void UpdateEntity_ShouldPreserveExistingValues_WhenNewValuesAreNull()
    {
        // Arrange
        var request = new UpdateTeamRequest
        {
            Name = null,
            Description = null
        };

        var existing = new TeamBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithName("Original Name")
            .WithDescription("Original Description")
            .WithIsActive(true)
            .Build();

        // Act
        var entity = request.UpdateEntity(existing);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(existing.Id);
        entity.Name.Should().Be(existing.Name);
        entity.Description.Should().Be(existing.Description);
        entity.IsActive.Should().Be(existing.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var entity = new TeamBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithName(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithIsActive(true)
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.Name.Should().Be(entity.Name);
        response.Description.Should().Be(entity.Description);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        // Arrange
        var team1 = new TeamBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var team2 = new TeamBuilder()
            .WithId(Guid.NewGuid())
            .WithName(_fixture.Create<string>())
            .WithDescription(_fixture.Create<string>())
            .WithIsActive(_fixture.Create<bool>())
            .Build();

        var entities = new List<TeamEntity> { team1, team2 };
        var pagedResult = new PagedResult<TeamEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

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
