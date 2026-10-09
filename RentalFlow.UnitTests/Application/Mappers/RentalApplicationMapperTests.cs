namespace RentalFlow.UnitTests.Application.Mappers;

public sealed class RentalApplicationMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapCorrectly()
    {
        var applicant = _testsFixtures.MakeApplicant();
        var property = _testsFixtures.MakeProperty();
        var @operator = _testsFixtures.MakeOperator();

        var request = _fixture.Build<AddRentalApplicationRequest>()
            .With(r => r.ApplicantId, applicant.Id)
            .With(r => r.PropertyId, property.Id)
            .With(r => r.OperatorId, @operator.Id)
            .Create();

        var entity = request.ToEntity(applicant, property, @operator);

        entity.Should().NotBeNull();
        entity.FinancedAmount.Should().Be(request.FinancedAmount);
        entity.TotalAmount.Should().Be(request.TotalAmount);
        entity.Installments.Should().Be(request.Installments);
        entity.ApplicantId.Should().Be(applicant.Id);
        entity.PropertyId.Should().Be(property.Id);
        entity.OperatorId.Should().Be(@operator.Id);
        entity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateFrom_ShouldMapAllFieldsCorrectly_WhenRequestHasValues()
    {
        var entity = _testsFixtures.MakeRentalApplication();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();

        entity.UpdateFrom(request);

        entity.FinancedAmount.Should().Be(request.FinancedAmount);
        entity.TotalAmount.Should().Be(request.TotalAmount);
        entity.Installments.Should().Be(request.Installments);
        entity.ContractDate.Should().Be(request.ContractDate);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = _testsFixtures.MakeRentalApplication();

        var request = _fixture.Build<UpdateRentalApplicationRequest>()
            .Without(r => r.FinancedAmount)
            .Without(r => r.TotalAmount)
            .Without(r => r.Installments)
            .Without(r => r.ContractDate)
            .Create();

        entity.UpdateFrom(request);

        entity.FinancedAmount.Should().Be(entity.FinancedAmount);
        entity.TotalAmount.Should().Be(entity.TotalAmount);
        entity.Installments.Should().Be(entity.Installments);
        entity.ContractDate.Should().Be(entity.ContractDate);
    }

    [Fact]
    public void ToResponse_ShouldMapCorrectly()
    {
        var entity = _testsFixtures.MakeRentalApplication();

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.FinancedAmount.Should().Be(entity.FinancedAmount);
        response.TotalAmount.Should().Be(entity.TotalAmount);
        response.Installments.Should().Be(entity.Installments);
        response.Status.Should().Be(entity.Status);
        response.ProposalNumber.Should().Be(entity.ProposalNumber);
        response.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var applicant = _testsFixtures.MakeApplicant();
        var property = _testsFixtures.MakeProperty();
        var @operator = _testsFixtures.MakeOperator();

        var entity = _testsFixtures.MakeRentalApplication(applicant: applicant, property: property, @operator: @operator);

        var response = entity.ToResponse();

        response.Should().NotBeNull();
        response.ApplicantId.Should().Be(applicant.Id);
        response.ApplicantName.Should().Be(applicant.FullName);
        response.ApplicantCpf.Should().Be(applicant.Cpf);
        response.PropertyId.Should().Be(property.Id);
        response.PropertyRentPrice.Should().Be(property.RentPrice);
        response.OperatorId.Should().Be(@operator.Id);
        response.OperatorName.Should().Be(@operator.Name);
    }

    [Fact]
    public void ToResponse_ShouldMapPagedResultToPagedResultResponse()
    {
        var entities = new List<RentalApplicationEntity>
        {
            _testsFixtures.MakeRentalApplication(),
            _testsFixtures.MakeRentalApplication()
        };

        var pagedResult = new PagedResult<RentalApplicationEntity>(entities, totalResults: 2, page: 1, pageSize: 60);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(ra => ra.ToResponse()));
    }
}