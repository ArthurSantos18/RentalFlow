using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Queries.Applicant;
using RentalFlow.Domain.Entities.Applicant;

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
        var query = new GetApplicantsQuery();

        var applicant1 = ApplicantEntity.Empty
            .SetCpf("52998224725");
        var applicant2 = ApplicantEntity.Empty
            .SetCpf("92281813037"); ;

        var applicants = new List<ApplicantEntity> { applicant1, applicant2 };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicants);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
