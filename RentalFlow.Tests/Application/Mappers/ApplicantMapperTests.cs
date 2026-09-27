namespace RentalFlow.Tests.Application.Mappers;

public sealed class ApplicantMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
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
        var entity = _testsFixtures.MakeApplicant();

        var request = _fixture.Create<UpdateApplicantRequest>();

        entity.UpdateFrom(request);

        entity.FullName.Should().Be(request.FullName);
        entity.Email.Should().Be(request.Email);
        entity.Phone.Should().Be(request.Phone);
        entity.MonthlyIncome.Should().Be(request.MonthlyIncome);
        entity.IsActive.Should().Be(request.IsActive!.Value);
        entity.Cpf.Should().Be(entity.Cpf);
    }

    [Fact]
    public void UpdateFrom_ShouldKeepExistingValues_WhenRequestFieldsAreNull()
    {
        var entity = _testsFixtures.MakeApplicant();

        var request = _fixture.Build<UpdateApplicantRequest>()
            .Without(r => r.FullName)
            .Without(r => r.Email)
            .Without(r => r.Phone)
            .Without(r => r.MonthlyIncome)
            .Without(r => r.IsActive)
            .Create();

        entity.UpdateFrom(request);

        entity.FullName.Should().Be(entity.FullName);
        entity.Email.Should().Be(entity.Email);
        entity.Phone.Should().Be(entity.Phone);
        entity.MonthlyIncome.Should().Be(entity.MonthlyIncome);
        entity.IsActive.Should().Be(entity.IsActive);
    }

    [Fact]
    public void UpdateFrom_ShouldClearPhone_WhenRequestPhoneIsWhitespace()
    {
        var entity = _testsFixtures.MakeApplicant();

        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Phone, "   ")
            .Create();

        entity.UpdateFrom(request);

        entity.Phone.Should().BeNull();
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        var entity = _testsFixtures.MakeApplicant();

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
            _testsFixtures.MakeApplicant(),
            _testsFixtures.MakeApplicant()
        };

        var pagedResult = new PagedResult<ApplicantEntity>(entities, totalResults: 10, page: 2, pageSize: 2);

        var response = pagedResult.ToResponse();

        response.Should().NotBeNull();
        response.Page.Should().Be(pagedResult.Page);
        response.PageSize.Should().Be(pagedResult.PageSize);
        response.TotalResults.Should().Be(pagedResult.TotalResults);
        response.Results.Should().HaveCount(pagedResult.Results.Count());
        response.Results.Should().BeEquivalentTo(entities.Select(e => e.ToResponse()));
    }
}