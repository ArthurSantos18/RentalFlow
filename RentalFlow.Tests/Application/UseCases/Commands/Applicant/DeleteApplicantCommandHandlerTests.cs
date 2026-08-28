using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Applicant;

public sealed class DeleteApplicantCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly DeleteApplicantCommandHandler _handler;

    public DeleteApplicantCommandHandlerTests()
    {
        _handler = new DeleteApplicantCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteApplicant_WhenApplicantExistsAndIsActive()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var command = _fixture.Build<DeleteApplicantCommand>()
            .With(c => c.Id, applicantId)
            .Create();

        var applicant = ApplicantEntity.Empty
            .SetId(applicantId)
            .SetIsActive(true);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(applicant), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenApplicantAlreadyInactive()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var command = _fixture.Build<DeleteApplicantCommand>()
            .With(c => c.Id, applicantId)
            .Create();

        var applicant = ApplicantEntity.Empty
            .SetId(applicantId)
            .SetIsActive(false);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<ApplicantEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantNotFound()
    {
        // Arrange
        var expectedError = PropertyErrors.PropertyNotFound;
        var command = _fixture.Create<DeleteApplicantCommand>();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<ApplicantEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
