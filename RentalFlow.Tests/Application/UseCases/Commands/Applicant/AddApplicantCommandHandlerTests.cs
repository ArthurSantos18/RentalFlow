namespace RentalFlow.Tests.Application.UseCases.Commands.Applicant;

public sealed class AddApplicantCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly Mock<ILogger<AddApplicantCommandHandler>> _loggerMock = new();
    private readonly AddApplicantCommandHandler _handler;

    public AddApplicantCommandHandlerTests()
    {
        _handler = new AddApplicantCommandHandler(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddApplicant_WhenCpfIsUnique()
    {
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "12345678900")
            .Create();

        var command = _fixture.Build<AddApplicantCommand>()
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicantEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConflict_WhenCpfAlreadyExists()
    {
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "12345678900")
            .Create();

        var command = _fixture.Build<AddApplicantCommand>()
            .With(c => c.Request, request)
            .Create();

        var existingApplicant = _testsFixtures.MakeApplicant(cpf: request.Cpf);

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingApplicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantAlreadyExists);

        _repositoryMock.Verify(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicantEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}