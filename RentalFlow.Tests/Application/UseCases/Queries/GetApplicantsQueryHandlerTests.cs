namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetApplicantsQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly GetApplicantsQueryHandler _handler;

    public GetApplicantsQueryHandlerTests()
    {
        _handler = new GetApplicantsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenApplicantsExist()
    {
        var request = _fixture.Create<GetApplicantRequest>();
        var query = _fixture.Build<GetApplicantsQuery>()
            .With(q => q.Request, request)
            .Create();

        var applicants = new List<ApplicantEntity>
        {
            TestsFixtures.MakeApplicant(fullName: "John Doe"),
            TestsFixtures.MakeApplicant(fullName: "Jane Doe", cpf: "11122233344", email: "jane@test.com")
        };

        var pagedResult = new PagedResult<ApplicantEntity>(
            results: applicants,
            totalResults: applicants.Count,
            page: 1,
            pageSize: 60);

        _repositoryMock
            .Setup(r => r.GetApplicantsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(60);
        result.Value.TotalResults.Should().Be(applicants.Count);
        result.Value.Results.Should().HaveCount(applicants.Count);
        result.Value.Results.Should().BeEquivalentTo(applicants.Select(a => a.ToResponse()));

        _repositoryMock.Verify(r => r.GetApplicantsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}