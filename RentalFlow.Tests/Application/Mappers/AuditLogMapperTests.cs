namespace RentalFlow.Tests.Application.Mappers;

public sealed class AuditLogMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = _fixture.Create<AuditLogEntity>();

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.EntityName.Should().Be(entity.EntityName);
        response.FieldName.Should().Be(entity.FieldName);
        response.NewValue.Should().Be(entity.NewValue);
        response.OldValue.Should().Be(entity.OldValue);
        response.UserName.Should().Be(entity.UserName);
        response.Action.Should().Be(entity.Action);
        response.CreatedAt.Should().Be(entity.CreatedAt);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = _fixture.CreateMany<AuditLogEntity>(2).ToList();

        var pagedResult = new PagedResult<AuditLogEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}