using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Domain.Entities.Applicant;

namespace RentalFlow.Tests.Application.UseCases.Commands.Applicant;

public sealed class UpdateApplicantCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly UpdateApplicantCommandHandler _handler;

    public UpdateApplicantCommandHandlerTests()
    {
        _handler = new UpdateApplicantCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateApplicant_WhenExists()
    {
        // Arrange
        var applicantId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateApplicantRequest>();
        var command = _fixture.Build<UpdateApplicantCommand>()
            .With(c => c.Id, applicantId)
            .With(c => c.Request, request)
            .Create();

        var existingApplicant = ApplicantEntity.Empty
            .SetId(applicantId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingApplicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<ApplicantEntity>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenApplicantDoesNotExist()
    {
        // Arrange
        var applicantId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateApplicantRequest>();
        var command = _fixture.Build<UpdateApplicantCommand>()
            .With(c => c.Id, applicantId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock.Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<ApplicantEntity>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
