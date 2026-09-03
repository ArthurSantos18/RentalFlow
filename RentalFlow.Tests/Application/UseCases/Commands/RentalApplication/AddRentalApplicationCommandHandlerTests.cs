using AutoFixture;
using FluentAssertions;
using Moq;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.RentalApplication;
using RentalFlow.Application.UseCases.Commands.RentalApplication;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Entities.Property;
using RentalFlow.Domain.Entities.RentalApplication;
using RentalFlow.Domain.Errors;

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
            _operatorRepoMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldAddRentalApplication_WhenAllDependenciesExist()
    {
        // Arrange
        var applicant = ApplicantEntity.Empty.SetId(Guid.NewGuid());
        var property = PropertyEntity.Empty.SetId(Guid.NewGuid());
        var @operator = OperatorEntity.Empty.SetId(Guid.NewGuid());

        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
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
        // Arrange
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>())).ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _applicantRepoMock.Verify(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPropertyNotFound()
    {
        // Arrange
        var applicant = ApplicantEntity.Empty.SetId(Guid.NewGuid());
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _propertyRepoMock.Verify(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _propertyRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenOperatorNotFound()
    {
        // Arrange
        var applicant = ApplicantEntity.Empty.SetId(Guid.NewGuid());
        var property = PropertyEntity.Empty.SetId(Guid.NewGuid());
        var request = _fixture.Create<AddRentalApplicationRequest>();
        var command = _fixture.Build<AddRentalApplicationCommand>()
            .With(c => c.Request, request)
            .Create();

        _applicantRepoMock
            .Setup(r => r.GetByIdAsync(request.ApplicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        _propertyRepoMock
            .Setup(r => r.GetByIdAsync(request.PropertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _operatorRepoMock
            .Setup(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _operatorRepoMock.Verify(r => r.GetByIdAsync(request.OperatorId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalRepoMock.Verify(r => r.AddAsync(It.IsAny<RentalApplicationEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        _rentalRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _operatorRepoMock.VerifyNoOtherCalls();
        _rentalRepoMock.VerifyNoOtherCalls();
    }
}
