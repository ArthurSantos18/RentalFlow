using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

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
    public async Task HandleAsync_ShouldSoftDeleteApplicant_WhenApplicantExists()
    {
        var applicantId = Guid.NewGuid();
        var command = _fixture.Build<DeleteApplicantCommand>()
            .With(c => c.Id, applicantId)
            .Create();

        var applicant = TestFixtures.MakeApplicant(id: applicantId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        applicant.IsDeleted.Should().BeTrue();
        applicant.DeletedAt.Should().NotBeNull();
        applicant.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantNotFound()
    {
        var command = _fixture.Create<DeleteApplicantCommand>();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}