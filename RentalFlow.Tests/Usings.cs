global using AutoFixture;

global using FluentAssertions;
global using FluentValidation.TestHelper;

global using Moq;

global using System.Linq.Expressions;

global using RentalFlow.Application.Interfaces.Repositories;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.Requests.Team;
global using RentalFlow.Application.Requests.User;

global using RentalFlow.Application.UseCases.Commands.Applicant;
global using RentalFlow.Application.UseCases.Commands.Operator;
global using RentalFlow.Application.UseCases.Commands.Property;
global using RentalFlow.Application.UseCases.Commands.RentalApplication;
global using RentalFlow.Application.UseCases.Commands.Team;

global using RentalFlow.Application.UseCases.Queries.Applicant;
global using RentalFlow.Application.UseCases.Queries.Operator;
global using RentalFlow.Application.UseCases.Queries.Property;
global using RentalFlow.Application.UseCases.Queries.RentalApplication;
global using RentalFlow.Application.UseCases.Queries.Team;

global using RentalFlow.Application.Validators.Applicant;
global using RentalFlow.Application.Validators.Operator;
global using RentalFlow.Application.Validators.Property;
global using RentalFlow.Application.Validators.RentalApplication;
global using RentalFlow.Application.Validators.Team;

global using RentalFlow.Application.Responses;
global using RentalFlow.Application.Mappers;

global using RentalFlow.Domain.Entities;
global using RentalFlow.Domain.ValueObject;
global using RentalFlow.Domain.Enums;
global using RentalFlow.Domain.Patterns.PagedResult;
global using RentalFlow.Domain.Errors;

global using RentalFlow.Tests.Fixtures;