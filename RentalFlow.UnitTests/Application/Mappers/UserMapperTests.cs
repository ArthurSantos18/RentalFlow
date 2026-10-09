namespace RentalFlow.UnitTests.Application.Mappers;

public sealed class UserMapperTests
{
    private readonly Fixture _fixture = new();
    private readonly TestsFixtures _testsFixtures = new(new Fixture());

    [Fact]
    public void ToEntity_ShouldMapRequestToUserEntity()
    {
        var request = _fixture.Create<AddOperatorRequest>();
        var passwordHash = _fixture.Create<string>();
        var @operator = _testsFixtures.MakeOperator();

        var result = request.ToEntity(passwordHash, @operator);

        result.Email.Should().Be(request.Email.ToLowerInvariant());
        result.PasswordHash.Should().Be(passwordHash);
        result.MustChangePassword.Should().BeTrue();
        result.Operator.Should().Be(@operator);
        result.OperatorId.Should().Be(@operator.Id);
    }
}