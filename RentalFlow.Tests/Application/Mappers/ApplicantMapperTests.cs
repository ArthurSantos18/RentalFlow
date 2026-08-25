using AutoFixture;
using FluentAssertions;
using RentalFlow.Application.Mappers;
using RentalFlow.Application.Requests;
using RentalFlow.Domain.Entities.Applicant;

namespace RentalFlow.Tests.Application.Mappers;

public sealed class ApplicantMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToEntity_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<AddApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .With(r => r.MonthlyIncome, _fixture.Create<decimal>())
            .Create();

        // Act
        var entity = request.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeEmpty();
        entity.FullName.Should().Be(request.FullName);
        entity.Cpf.Should().Be(request.Cpf);
        entity.Email.Should().Be(request.Email);
        entity.Phone.Should().Be(request.Phone);
        entity.MonthlyIncome.Should().Be(request.MonthlyIncome);
        entity.IsActive.Should().BeTrue();
        entity.Applications.Should().BeEmpty();
    }

    [Fact]
    public void ToDomain_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var request = _fixture.Build<UpdateApplicantRequest>()
            .With(r => r.Cpf, "52998224725")
            .With(r => r.MonthlyIncome, _fixture.Create<decimal>())
            .Create();

        // Act
        var update = request.ToDomain();

        // Assert
        update.Should().NotBeNull();
        update.FullName.Should().Be(request.FullName);
        update.Cpf.Should().Be(request.Cpf);
        update.Email.Should().Be(request.Email);
        update.Phone.Should().Be(request.Phone);
        update.MonthlyIncome.Should().Be(request.MonthlyIncome);
    }

    [Fact]
    public void ToResponse_ShouldMapEntityToResponse()
    {
        // Arrange
        var entity = new ApplicantBuilder()
            .WithId(_fixture.Create<Guid>())
            .WithFullName(_fixture.Create<string>())
            .WithCpf("52998224725")
            .WithEmail(_fixture.Create<string>())
            .WithPhone(_fixture.Create<string>())
            .WithMonthlyIncome(_fixture.Create<decimal>())
            .WithActive(true)
            .Build();

        // Act
        var response = entity.ToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(entity.Id);
        response.FullName.Should().Be(entity.FullName);
        response.Cpf.Should().Be(entity.Cpf);
        response.Email.Should().Be(entity.Email);
        response.Phone.Should().Be(entity.Phone);
        response.MonthlyIncome.Should().Be(entity.MonthlyIncome);
    }

    [Fact]
    public void ToResponse_ShouldMapListOfEntitiesToResponses()
    {
        // Arrange
        var entities = new List<ApplicantEntity>
        {
            new ApplicantBuilder()
                .WithId(_fixture.Create<Guid>())
                .WithFullName(_fixture.Create<string>())
                .WithCpf("52998224725")
                .WithEmail(_fixture.Create<string>())
                .WithPhone(_fixture.Create<string>())
                .WithMonthlyIncome(_fixture.Create<decimal>())
                .WithActive(true)
                .Build(),

            new ApplicantBuilder()
                .WithId(_fixture.Create<Guid>())
                .WithFullName(_fixture.Create<string>())
                .WithCpf("92281813037")
                .WithEmail(_fixture.Create<string>())
                .WithPhone(_fixture.Create<string>())
                .WithMonthlyIncome(_fixture.Create<decimal>())
                .WithActive(true)
                .Build()
        };

        // Act
        var responses = entities.ToResponse();

        // Assert
        responses.Should().NotBeNull();
        responses.Should().HaveCount(entities.Count);
    }
}
