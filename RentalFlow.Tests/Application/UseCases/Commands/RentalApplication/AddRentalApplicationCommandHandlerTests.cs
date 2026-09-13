using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities;
using RentalFlow.Domain.Errors;
using RentalFlow.Tests.Fixtures;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class AddRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _rentalRepoMock = new();
    private readonly Mock<IApplicantRepository> _applicantRepoMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepoMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepoMock = new();
    private readonly AddRentalApplicationCommandHandler _handler;

    public AddRentalApplicationCommandHandlerTests()
    {
        _handler = new AddRentalApplicationCommandHandler(
            _rentalRepoMock.Object,
            _applicantRepoMock.Object,
            _propertyRepoMock.Object,
            _operatorRepoMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenAllDependenciesExist()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = TestFixtures.MakeApplicant(id: request.ApplicantId);
        var property = TestFixtures.MakeProperty(id: request.PropertyId, isAvailable: true);
        var @operator = TestFixtures.MakeOperator(id: request.OperatorId);

        _applicantRepoMock.Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>())).ReturnsAsync(applicant);
        _propertyRepoMock.Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>())).ReturnsAsync(property);
        _operatorRepoMock.Setup(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>())).ReturnsAsync(@operator);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _operatorRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenApplicantNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _operatorRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = TestFixtures.MakeApplicant(id: request.ApplicantId);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _operatorRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorNotFound()
    {
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        var applicant = TestFixtures.MakeApplicant(id: request.ApplicantId);
        var property = TestFixtures.MakeProperty(id: request.PropertyId, isAvailable: true);

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        var result = await _handler.HandleAsync(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepoMock.VerifyNoOtherCalls();
        _propertyRepoMock.VerifyNoOtherCalls();
        _operatorRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }
}