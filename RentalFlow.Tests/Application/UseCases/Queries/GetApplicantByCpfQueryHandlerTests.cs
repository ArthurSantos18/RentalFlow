using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.UseCases.Queries.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Queries;

public sealed class GetApplicantByCpfQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IApplicantRepository> _repositoryMock = new();
    private readonly GetApplicantByCpfQueryHandler _handler;

    public GetApplicantByCpfQueryHandlerTests()
    {
        _handler = new GetApplicantByCpfQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenApplicantExists()
    {
        // Arrange
        var cpf = "52998224725";
        var request = _fixture.Build<GetApplicantByCpfRequest>()
            .With(r => r.Cpf, "52998224725")
            .Create();

        var query = _fixture.Build<GetApplicantByCpfQuery>()
            .With(q => q.Request, request)
            .Create();

        var applicant = ApplicantEntity.Empty
            .SetCpf(cpf);

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantNotFound()
    {
        // Arrange
        var expectedError = ApplicantErrors.ApplicantNotFound;

        var request = _fixture.Build<GetApplicantByCpfRequest>()
            .With(r => r.Cpf, "52998224725")
            .Create();

        var query = _fixture.Build<GetApplicantByCpfQuery>()
            .With(q => q.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _repositoryMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }
}
