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
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Errors;

namespace RentalFlow.Tests.Application.UseCases.Commands.RentalApplication;

public sealed class UpdateRentalApplicationCommandHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IRentalApplicationRepository> _rentalApplicationRepositoryMock = new();
    private readonly Mock<IApplicantRepository> _applicantRepositoryMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock = new();
    private readonly Mock<IOperatorRepository> _operatorRepositoryMock = new();
    private readonly UpdateRentalApplicationCommandHandler _handler;

    public UpdateRentalApplicationCommandHandlerTests()
    {
        _handler = new UpdateRentalApplicationCommandHandler(_rentalApplicationRepositoryMock.Object,
            _applicantRepositoryMock.Object,
            _operatorRepositoryMock.Object,
            _propertyRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateRentalApplication_WhenExists()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = new UpdateRentalApplicationRequest
        {
            Installments = 24,
            FinancedAmount = 50000,
            TotalAmount = 60000,
            ContractDate = DateTime.UtcNow,
            ApplicantId = null,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var request = _fixture.Create<UpdateRentalApplicationRequest>();
        var command = new UpdateRentalApplicationCommand(id, request);

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalApplicationEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public async Task HandleAsync_ShouldReturnApplicantChangeNotAllowed_WhenApplicantIsChangedInInvalidStatus(RentalStatus status)
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var applicantId = _fixture.Create<Guid>();

        var applicant = ApplicantBuilder.Create()
            .WithId(applicantId)
            .WithActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicantId,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(status)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(a => a.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationApplicantChangeNotAllowed);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _applicantRepositoryMock.Verify(a => a.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantNotFound_WhenApplicantDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var applicantId = _fixture.Create<Guid>();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicantId,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicantEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicantId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantIsInactive_WhenApplicantIsInactive()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var applicant = ApplicantBuilder.Create()
            .WithActive(false)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApplicantErrors.ApplicantIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnApplicantAlreadyAssigned_WhenApplicantIsAlreadyAssigned()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var applicant = ApplicantBuilder.Create()
            .WithActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .WithApplicant(applicant)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.ApplicantAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateApplicant_WhenApplicantIsValid()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var applicant = ApplicantBuilder.Create()
            .WithActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = applicant.Id,
            OperatorId = null,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _applicantRepositoryMock
            .Setup(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(applicant);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existing.ApplicantId.Should().Be(applicant.Id);
        existing.Applicant.Should().Be(applicant);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _applicantRepositoryMock.Verify(r => r.GetByIdAsync(applicant.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public async Task HandleAsync_ShouldReturnOperatorChangeNotAllowed_WhenOperatorIsChangedInInvalidStatus(RentalStatus status)
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var operatorId = _fixture.Create<Guid>();

        var operatorEntity = OperatorBuilder.Create()
            .WithId(operatorId)
            .WithIsActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = operatorEntity.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(status)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(operatorEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatorEntity);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationOperatorChangeNotAllowed);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorNotFound_WhenOperatorDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var operatorId = _fixture.Create<Guid>();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = operatorId,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OperatorEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(operatorId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorIsInactive_WhenOperatorIsInactive()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var @operator = OperatorBuilder.Create()
            .WithIsActive(false)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OperatorErrors.OperatorIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOperatorAlreadyAssigned_WhenOperatorIsAlreadyAssigned()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var @operator = OperatorBuilder.Create()
            .WithIsActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .WithOperator(@operator)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.OperatorAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOperator_WhenOperatorIsValid()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var @operator = OperatorBuilder.Create()
            .WithIsActive(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = @operator.Id,
            PropertyId = null
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _operatorRepositoryMock
            .Setup(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existing.OperatorId.Should().Be(@operator.Id);
        existing.Operator.Should().Be(@operator);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _operatorRepositoryMock.Verify(r => r.GetByIdAsync(@operator.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(RentalStatus.Approved)]
    [InlineData(RentalStatus.Rejected)]
    public async Task HandleAsync_ShouldReturnPropertyChangeNotAllowed_WhenPropertyIsChangedInInvalidStatus(RentalStatus status)
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var propertyId = _fixture.Create<Guid>();

        var property = PropertyBuilder.Create()
            .WithId(propertyId)
            .WithIsActive(true)
            .WithIsAvailable(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(status)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.RentalApplicationPropertyChangeNotAllowed);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _propertyRepositoryMock.Verify(p => p.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotFound_WhenPropertyDoesNotExist()
    {
        // Arrange
        var id = _fixture.Create<Guid>();
        var propertyId = _fixture.Create<Guid>();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = propertyId
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyEntity?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotFound);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyIsInactive_WhenPropertyIsInactive()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var property = PropertyBuilder.Create()
            .WithIsActive(false)
            .WithIsAvailable(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyIsInactive);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyNotAvailable_WhenPropertyIsNotAvailable()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var property = PropertyBuilder.Create()
            .WithIsActive(true)
            .WithIsAvailable(false)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PropertyErrors.PropertyNotAvailable);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPropertyAlreadyAssigned_WhenPropertyIsAlreadyAssigned()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var property = PropertyBuilder.Create()
            .WithIsActive(true)
            .WithIsAvailable(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .WithProperty(property)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RentalApplicationErrors.PropertyAlreadyAssigned);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(It.IsAny<RentalApplicationEntity>()), Times.Never);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProperty_WhenPropertyIsValid()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        var property = PropertyBuilder.Create()
            .WithIsActive(true)
            .WithIsAvailable(true)
            .Build();

        var request = new UpdateRentalApplicationRequest
        {
            ApplicantId = null,
            OperatorId = null,
            PropertyId = property.Id
        };

        var command = new UpdateRentalApplicationCommand(id, request);

        var existing = RentalApplicationBuilder.Create()
            .WithId(id)
            .WithStatus(RentalStatus.Draft)
            .Build();

        _rentalApplicationRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existing.PropertyId.Should().Be(property.Id);
        existing.Property.Should().Be(property);

        _rentalApplicationRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _propertyRepositoryMock.Verify(r => r.GetByIdAsync(property.Id, It.IsAny<CancellationToken>()), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.Update(existing), Times.Once);
        _rentalApplicationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _applicantRepositoryMock.VerifyNoOtherCalls();
        _operatorRepositoryMock.VerifyNoOtherCalls();
        _propertyRepositoryMock.VerifyNoOtherCalls();
        _rentalApplicationRepositoryMock.VerifyNoOtherCalls();
    }
}