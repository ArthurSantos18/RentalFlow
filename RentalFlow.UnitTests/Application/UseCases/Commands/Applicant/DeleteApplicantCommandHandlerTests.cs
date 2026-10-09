namespace RentalFlow.UnitTests.Application.UseCases.Commands.Applicant;

public sealed class DeleteApplicantCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IApplicantRepository> _applicantRepositoryMock = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<ILogger<DeleteApplicantCommandHandler>> _loggerMock = new();
    private readonly DeleteApplicantCommandHandler _handler;

    public DeleteApplicantCommandHandlerTests()
    {
        _handler = new DeleteApplicantCommandHandler(
            _applicantRepositoryMock.Object,
            _rentalApplicationRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantNotFound_WhenApplicantDoesNotExist()
    {
        var command = _fixture.Create<DeleteApplicantCommand>();

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.ApplicantHasApplicationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantHasApplications_WhenApplicantHasApplications()
    {
        var applicant = _testsFixtures.MakeApplicant();
        var command = _fixture.Build<DeleteApplicantCommand>()
            .With(c => c.Id, applicant.Id)
            .Create();

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _rentalApplicationRepositoryMock
            .Setup(r => r.ApplicantHasApplicationsAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantHasApplications);

        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.ApplicantHasApplicationsAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteApplicant_WhenApplicantHasNoApplications()
    {
        var applicant = _testsFixtures.MakeApplicant(isActive: false);
        var command = _fixture.Build<DeleteApplicantCommand>()
            .With(c => c.Id, applicant.Id)
            .Create();

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _rentalApplicationRepositoryMock
            .Setup(r => r.ApplicantHasApplicationsAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _applicantRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        applicant.IsActive.Should().BeFalse();

        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.ApplicantHasApplicationsAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }
}