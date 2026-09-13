using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Application.UseCases.Commands.Applicant;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

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
        var applicantId = _fixture.Create<Guid>();

        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.FullName, "Updated Name")
            .With(r => r.Email, "updated@test.com")
            .With(r => r.Phone, "987654321")
            .With(r => r.MonthlyIncome, 7500m)
            .With(r => r.IsActive, true)
            .Create();

        var command = _fixture.Build<UpdateApplicantCommand>()
            .With(c => c.Id, applicantId)
            .With(c => c.Request, request)
            .Create();

        var existingApplicant = TestFixtures.MakeApplicant(
            id: applicantId,
            fullName: "Old Name",
            email: "old@test.com",
            phone: "123456789",
            monthlyIncome: 1000m);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingApplicant);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingApplicant.FullName.Should().Be("Updated Name");
        existingApplicant.Email.Should().Be("updated@test.com");
        existingApplicant.Phone.Should().Be("987654321");
        existingApplicant.MonthlyIncome.Should().Be(7500m);

        _repositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenApplicantDoesNotExist()
    {
        var applicantId = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateApplicantRequest>();

        var command = _fixture.Build<UpdateApplicantCommand>()
            .With(c => c.Id, applicantId)
            .With(c => c.Request, request)
            .Create();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _repositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _repositoryMock.VerifyNoOtherCalls();
    }
}