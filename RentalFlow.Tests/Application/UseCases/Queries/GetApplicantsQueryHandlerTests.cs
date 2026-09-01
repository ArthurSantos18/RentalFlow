using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.UseCases.Queries.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Patterns.PagedResult;

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
        // Arrange
        var request = _fixture.Create<GetApplicantRequest>();
        var query = _fixture.Build<GetApplicantsQuery>()
            .With(q => q.Request, request)
            .Create();

        var applicant = new ApplicantBuilder()
            .WithId(Guid.NewGuid())
            .WithFullName(_fixture.Create<string>())
            .WithCpf(_fixture.Create<string>().Substring(0, 11))
            .WithEmail(_fixture.Create<string>() + "@test.com")
            .WithPhone(_fixture.Create<string>().Substring(0, 11))
            .WithMonthlyIncome(_fixture.Create<decimal>())
            .WithActive(_fixture.Create<bool>())
            .Build();

        var applicants = new List<ApplicantEntity> { applicant };
        var pagedResult = new PagedResult<ApplicantEntity>(
            results: applicants,
            totalResults: 1,
            page: 1,
            pageSize: 60
        );

        _repositoryMock
            .Setup(r => r.GetApplicantsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetApplicantsAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}
