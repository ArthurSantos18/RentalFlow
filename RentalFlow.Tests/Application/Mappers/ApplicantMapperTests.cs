using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class ApplicantMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .With(r => r.MonthlyIncome, _fixture.Create<decimal>())
            .Create();

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.FullName.Should().Be(request.FullName);
        entity.Cpf.Should().Be(request.Cpf);
        entity.Email.Should().Be(request.Email);
        entity.Phone.Should().Be(request.Phone);
        entity.MonthlyIncome.Should().Be(request.MonthlyIncome);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }


    [Fact]
    public void UpdateEntity_ShouldMapAllFieldsCorrectly_WhenExistingProvided()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .With(r => r.MonthlyIncome, _fixture.Create<decimal>())
            .Create();

        var existing = new ApplicantBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithFullName("Existing Name")
            .WithCpf("11122233344")
            .WithEmail("existing@test.com")
            .WithPhone("123456789")
            .WithMonthlyIncome(100m)
            .WithActive(true)
            .Build();

        // Act
        var entity = request.UpdateEntity(existing);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(existing.Id);
        entity.FullName.Should().Be(request.FullName ?? existing.FullName);
        entity.Cpf.Should().Be(string.IsNullOrEmpty(request.Cpf) ? existing.Cpf : request.Cpf);
        entity.Email.Should().Be(request.Email ?? existing.Email);
        entity.Phone.Should().Be(request.Phone ?? existing.Phone);
        entity.MonthlyIncome.Should().Be(request.MonthlyIncome ?? existing.MonthlyIncome);
        entity.IsActive.Should().Be(existing.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var entity = new ApplicantBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithFullName(_fixture.Create<string>())
            .WithCpf("52998224725")
            .WithEmail(_fixture.Create<string>())
            .WithPhone(_fixture.Create<string>())
            .WithMonthlyIncome(_fixture.Create<decimal>())
            .WithActive(true)
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.FullName.Should().Be(entity.FullName);
        response.Cpf.Should().Be(entity.Cpf);
        response.Email.Should().Be(entity.Email);
        response.Phone.Should().Be(entity.Phone);
        response.MonthlyIncome.Should().Be(entity.MonthlyIncome);
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        // Arrange
        var entity1 = new ApplicantBuilder()
            .WithId(Guid.NewGuid())
            .WithFullName(_fixture.Create<string>())
            .WithCpf(_fixture.Create<string>().Substring(0, 11))
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithPhone(_fixture.Create<string>().Substring(0, 11))
            .WithMonthlyIncome(_fixture.Create<decimal>())
            .WithActive(_fixture.Create<bool>())
            .Build();

        var entity2 = new ApplicantBuilder()
            .WithId(Guid.NewGuid())
            .WithFullName(_fixture.Create<string>())
            .WithCpf(_fixture.Create<string>().Substring(0, 11))
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithPhone(_fixture.Create<string>().Substring(0, 11))
            .WithMonthlyIncome(_fixture.Create<decimal>())
            .WithActive(_fixture.Create<bool>())
            .Build();

        var entities = new List<ApplicantEntity> { entity1, entity2 };
        var pagedResult = new PagedResult<ApplicantEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

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
