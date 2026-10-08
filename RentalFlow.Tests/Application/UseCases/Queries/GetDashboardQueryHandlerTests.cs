namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetDashboardQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IReportRepository> _repositoryMock = new();
    private readonly GetDashboardQueryHandler _handler;

    public GetDashboardQueryHandlerTests()
    {
        _handler = new GetDashboardQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenAggregatesExist()
    {
        // Arrange
        var query = _fixture.Create<GetDashboardQuery>();

        var applicants = _fixture.Create<ApplicantAggregate>();
        var properties = _fixture.Create<PropertyAggregate>();
        var rentalApplications = _fixture.Create<RentalApplicationAggregate>();
        var operators = _fixture.Create<OperatorAggregate>();
        var teams = _fixture.Create<TeamAggregate>();

        _repositoryMock
            .Setup(r => r.GetApplicantAggregateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicants);

        _repositoryMock
            .Setup(r => r.GetPropertyAggregateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        _repositoryMock
            .Setup(r => r.GetRentalApplicationAggregateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentalApplications);

        _repositoryMock
            .Setup(r => r.GetOperatorAggregateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(operators);

        _repositoryMock
            .Setup(r => r.GetTeamAggregateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetApplicantAggregateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetPropertyAggregateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetRentalApplicationAggregateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetOperatorAggregateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetTeamAggregateAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}