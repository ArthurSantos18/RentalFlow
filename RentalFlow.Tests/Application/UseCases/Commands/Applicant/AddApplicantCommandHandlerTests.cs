using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.Applicant;

public sealed class AddApplicantCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly AddApplicantCommandHandler _handler;

    public AddApplicantCommandHandlerTests()
    {
        _handler = new AddApplicantCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddApplicant_WhenCpfIsUnique()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>().With(r => r.Cpf, "12345678900").Create();
        var command = _fixture.Build<AddApplicantCommand>().With(r => r.Request, request).Create();

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicantEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConflict_WhenCpfAlreadyExists()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>().With(r => r.Cpf, "12345678900").Create();
        var command = _fixture.Build<AddApplicantCommand>().With(r => r.Request, request).Create();

        var existingApplicant = ApplicantEntity.Empty.SetCpf(request.Cpf);

        var error = ApplicantErrors.ApplicantDoesExist;

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(request.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingApplicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);

        _repositoryMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicantEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
