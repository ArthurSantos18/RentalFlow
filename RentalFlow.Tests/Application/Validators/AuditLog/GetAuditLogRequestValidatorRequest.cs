namespace RentalFlow.Tests.Application.Validators.AuditLog;

public sealed class GetAuditLogRequestValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetAuditLogRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldHaveError_WhenEntityNameIsInvalid()
    {
        var request = _fixture.Build<GetAuditLogRequest>()
            .With(r => r.EntityName, (AuditEntity)999)
            .With(r => r.EntityId, Guid.NewGuid())
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntityName)
            .WithErrorMessage("O nome da entidade é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEntityIdIsEmpty()
    {
        var request = _fixture.Build<GetAuditLogRequest>()
            .With(r => r.EntityName, AuditEntity.ApplicantEntity)
            .With(r => r.EntityId, Guid.Empty)
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.EntityId)
            .WithErrorMessage("O ID da entidade é obrigatório.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenRequestIsValid()
    {
        var request = _fixture.Build<GetAuditLogRequest>()
            .With(r => r.EntityName, AuditEntity.ApplicantEntity)
            .With(r => r.EntityId, Guid.NewGuid())
            .Create();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}