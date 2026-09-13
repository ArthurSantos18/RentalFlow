using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Tests.Fixtures;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class ApplicantMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .With(r => r.MonthlyIncome, 5000m)
            .Create();

        var entity = request.ToEntity();

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
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = TestFixtures.MakeApplicant(
            fullName: "Existing Name",
            cpf: "11122233344",
            email: "existing@test.com",
            phone: "123456789",
            monthlyIncome: 100m);

        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, "New Name")
            .With(r => r.Email, "new@test.com")
            .With(r => r.Phone, "987654321")
            .With(r => r.MonthlyIncome, 200m)
            .With(r => r.IsActive, false)
            .Create();

        entity.UpdateFrom(request);

        entity.FullName.Should().Be("New Name");
        entity.Email.Should().Be("new@test.com");
        entity.Phone.Should().Be("987654321");
        entity.MonthlyIncome.Should().Be(200m);
        entity.IsActive.Should().BeFalse();
        entity.Cpf.Should().Be("11122233344");
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = TestFixtures.MakeApplicant(
            fullName: "Existing Name",
            cpf: "11122233344",
            email: "existing@test.com",
            phone: "123456789",
            monthlyIncome: 100m);

        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, (string?)null)
            .With(r => r.Email, (string?)null)
            .With(r => r.Phone, (string?)null)
            .With(r => r.MonthlyIncome, (decimal?)null)
            .With(r => r.IsActive, (bool?)null)
            .Create();

        entity.UpdateFrom(request);

        entity.FullName.Should().Be("Existing Name");
        entity.Email.Should().Be("existing@test.com");
        entity.Phone.Should().Be("123456789");
        entity.MonthlyIncome.Should().Be(100m);
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateFrom_ShouldClearPhone_WhenRequestPhoneIsWhitespace()
    {
        var entity = TestFixtures.MakeApplicant(phone: "123456789");

        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, "   ")
            .Create();

        entity.UpdateFrom(request);

        entity.Phone.Should().BeNull();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = TestFixtures.MakeApplicant();

        var response = entity.ToResponse();

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
        var entities = new List<ApplicantEntity>
        {
            TestFixtures.MakeApplicant(fullName: "John Doe"),
            TestFixtures.MakeApplicant(fullName: "Jane Doe", cpf: "11122233344", email: "jane@test.com")
        };

        var pagedResult = new PagedResult<ApplicantEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(2);
        response.PageSize.Should().Be(2);
        response.TotalResults.Should().Be(10);
        response.Results.Should().HaveCount(2);
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}