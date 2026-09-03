using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Patterns.PagedResult;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class RentalApplicationMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapCorrectly()
    {
        var applicant = ApplicantEntity.Empty.SetId(Guid.NewGuid());
        var property = PropertyEntity.Empty.SetId(Guid.NewGuid());
        var @operator = OperatorEntity.Empty.SetId(Guid.NewGuid());

        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .With(r => r.PropertyId, property.Id)
            .With(r => r.OperatorId, @operator.Id)
            .With(r => r.FinancedAmount, 100m)
            .With(r => r.TotalAmount, 200m)
            .With(r => r.Installments, 12)
            .Create();

        var entity = request.ToEntity(applicant, property, @operator);

        entity.FinancedAmount.Should().Be(request.FinancedAmount);
        entity.TotalAmount.Should().Be(request.TotalAmount);
        entity.Installments.Should().Be(request.Installments);
        entity.ApplicantId.Should().Be(applicant.Id);
        entity.PropertyId.Should().Be(property.Id);
        entity.OperatorId.Should().Be(@operator.Id);
    }

    [Fact]
    public void ToUpdateDomain_ShouldHandleNullValues()
    {
        // Arrange
        var request = new UpdateRentalApplicationRequest();

        var existing = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(100m)
            .WithTotalAmount(200m)
            .WithInstallments(12)
            .WithStatus(RentalFlow.Domain.Enums.RentalStatus.Draft)
            .WithContractDate(DateTime.UtcNow)
            .WithIsActive(true)
            .Build();

        // Act
        var entity = request.ToEntity(existing);

        // Assert
        entity.Should().NotBeNull();
        entity.FinancedAmount.Should().Be(existing.FinancedAmount);
        entity.TotalAmount.Should().Be(existing.TotalAmount);
        entity.Installments.Should().Be(existing.Installments);
        entity.Status.Should().Be(existing.Status);
        entity.ContractDate.Should().Be(existing.ContractDate);
        entity.IsActive.Should().Be(existing.IsActive);
    }

    [Fact]
    public void ToUpdateDomain_ShouldMapCorrectly()
    {
        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .With(r => r.FinancedAmount, 100m)
            .With(r => r.TotalAmount, 200m)
            .With(r => r.Installments, 12)
            .Create();

        var existing = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(50m)
            .WithTotalAmount(80m)
            .WithInstallments(6)
            .Build();

        var entity = request.ToEntity(existing);

        entity.FinancedAmount.Should().Be(request.FinancedAmount);
        entity.TotalAmount.Should().Be(request.TotalAmount);
        entity.Installments.Should().Be(request.Installments);
    }

    [Fact]
    public void ToResponse_ShouldMapCorrectly()
    {
        var entity = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(100m)
            .WithTotalAmount(200m)
            .WithInstallments(12)
            .Build();

        var response = entity.ToResponse();

        response.Id.Should().Be(entity.Id);
        response.FinancedAmount.Should().Be(entity.FinancedAmount);
        response.TotalAmount.Should().Be(entity.TotalAmount);
        response.Installments.Should().Be(entity.Installments);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var applicant = ApplicantEntity.Empty.SetId(Guid.NewGuid()).SetFullName("John Doe").SetCpf("52998224725");
        var property = PropertyEntity.Empty.SetId(Guid.NewGuid()).SetAddress(null!).SetRentPrice(150m);
        var @operator = OperatorEntity.Empty.SetId(Guid.NewGuid()).SetName("Operator 1");

        var entity = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(100m)
            .WithTotalAmount(200m)
            .WithInstallments(12)
            .WithApplicant(applicant)
            .WithProperty(property)
            .WithOperator(@operator)
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.FinancedAmount.Should().Be(entity.FinancedAmount);
        response.TotalAmount.Should().Be(entity.TotalAmount);
        response.Installments.Should().Be(entity.Installments);
        response.ApplicantId.Should().Be(entity.ApplicantId);
        response.ApplicantName.Should().Be(entity.Applicant?.FullName ?? string.Empty);
        response.ApplicantCpf.Should().Be(entity.Applicant?.Cpf ?? string.Empty);
        response.PropertyId.Should().Be(entity.PropertyId);
        response.PropertyAddress.Should().Be(entity.Property?.Address?.Street ?? string.Empty);
        response.PropertyRentPrice.Should().Be(entity.Property?.RentPrice ?? 0);
        response.OperatorId.Should().Be(entity.OperatorId);
        response.OperatorName.Should().Be(entity.Operator?.Name ?? string.Empty);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        // Arrange
        var entity1 = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(100m)
            .WithTotalAmount(200m)
            .WithInstallments(12)
            .Build();

        var entity2 = new RentalApplicationBuilder()
            .WithId(Guid.NewGuid())
            .WithFinancedAmount(150m)
            .WithTotalAmount(300m)
            .WithInstallments(24)
            .Build();

        var entities = new List<RentalApplicationEntity> { entity1, entity2 };
        var pagedResult = new PagedResult<RentalApplicationEntity>(entities, totalResults: 2, page: 1, pageSize: 60);

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
