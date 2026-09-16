global using Scalar.AspNetCore;

global using LiteBus.Commands.Abstractions;
global using LiteBus.Queries.Abstractions;

global using Microsoft.AspNetCore.Mvc;

global using RentalFlow.Crosscutting.Extensions;

global using RentalFlow.API.Helpers;

global using RentalFlow.Domain.Patterns.Result;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.UseCases.Commands.Applicant;
global using RentalFlow.Application.UseCases.Queries.Applicant;

global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.UseCases.Commands.Operator;
global using RentalFlow.Application.UseCases.Queries.Operator;

global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.UseCases.Commands.RentalApplication;
global using RentalFlow.Application.UseCases.Queries.RentalApplication;

global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.UseCases.Commands.Property;
global using RentalFlow.Application.UseCases.Queries.Property;

global using RentalFlow.Application.Requests.Team;
global using RentalFlow.Application.UseCases.Commands.Team;
global using RentalFlow.Application.UseCases.Queries.Team;