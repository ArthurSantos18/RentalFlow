using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Tests.Fixtures;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class RentalApplicationMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapCorrectly()
    {
        var applicant = TestFixtures.MakeApplicant();
        var property = TestFixtures.MakeProperty();
        var @operator = TestFixtures.MakeOperator();

        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .With(r => r.PropertyId, property.Id)
            .With(r => r.OperatorId, @operator.Id)
            .With(r => r.FinancedAmount, 100m)
            .With(r => r.TotalAmount, 200m)
            .With(r => r.Installments, 12)
            .With(r => r.ContractDate, (DateTime?)null)
            .Create();

        var entity = request.ToEntity(applicant, property, @operator);

        entity.Should().NotBeNull();
        entity.FinancedAmount.Should().Be(100m);
        entity.TotalAmount.Should().Be(200m);
        entity.Installments.Should().Be(12);
        entity.ApplicantId.Should().Be(applicant.Id);
        entity.PropertyId.Should().Be(property.Id);
        entity.OperatorId.Should().Be(@operator.Id);
        entity.ProposalNumber.Should().NotBeNullOrEmpty();
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = TestFixtures.MakeRentalApplication();
        var newDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 500m)
            .With(r => r.TotalAmount, 1000m)
            .With(r => r.Installments, 24)
            .With(r => r.ContractDate, newDate)
            .Create();

        entity.UpdateFrom(request);

        entity.FinancedAmount.Should().Be(500m);
        entity.TotalAmount.Should().Be(1000m);
        entity.Installments.Should().Be(24);
        entity.ContractDate.Should().Be(newDate);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var originalDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var entity = TestFixtures.MakeRentalApplication(
            financedAmount: 100m,
            totalAmount: 200m,
            installments: 12,
            contractDate: originalDate);

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, (decimal?)null)
            .With(r => r.TotalAmount, (decimal?)null)
            .With(r => r.Installments, (int?)null)
            .With(r => r.ContractDate, (DateTime?)null)
            .Create();

        entity.UpdateFrom(request);

        entity.FinancedAmount.Should().Be(100m);
        entity.TotalAmount.Should().Be(200m);
        entity.Installments.Should().Be(12);
        entity.ContractDate.Should().Be(originalDate);
    }

    [Fact]
    public void ToResponse_ShouldMapCorrectly()
    {
        var entity = TestFixtures.MakeRentalApplication(
            financedAmount: 100m,
            totalAmount: 200m,
            installments: 12);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.FinancedAmount.Should().Be(100m);
        response.TotalAmount.Should().Be(200m);
        response.Installments.Should().Be(12);
        response.Status.Should().Be(entity.Status);
        response.ProposalNumber.Should().Be(entity.ProposalNumber);
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var applicant = TestFixtures.MakeApplicant(fullName: "John Doe", cpf: "52998224725");
        var property = TestFixtures.MakeProperty(rentPrice: 150m);
        var @operator = TestFixtures.MakeOperator(name: "Operator 1");

        var entity = TestFixtures.MakeRentalApplication(
            applicant: applicant,
            property: property,
            @operator: @operator);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.ApplicantId.Should().Be(applicant.Id);
        response.ApplicantName.Should().Be("John Doe");
        response.ApplicantCpf.Should().Be("52998224725");
        response.PropertyId.Should().Be(property.Id);
        response.PropertyRentPrice.Should().Be(150m);
        response.OperatorId.Should().Be(@operator.Id);
        response.OperatorName.Should().Be("Operator 1");
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<RentalApplicationEntity>
        {
            TestFixtures.MakeRentalApplication(financedAmount: 100m),
            TestFixtures.MakeRentalApplication(financedAmount: 150m, installments: 24)
        };

        var pagedResult = new PagedResult<RentalApplicationEntity>(entities, totalResults: 2, page: 1, pageSize: 60);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(60);
        response.TotalResults.Should().Be(2);
        response.Results.Should().HaveCount(2);
    }
}