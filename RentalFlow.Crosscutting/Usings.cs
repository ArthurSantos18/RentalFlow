global using LiteBus.Commands;
global using LiteBus.Messaging;
global using LiteBus.Queries;

global using LiteBus.Extensions.Microsoft.DependencyInjection;

global using Microsoft.EntityFrameworkCore;

global using FluentValidation;
global using FluentValidation.AspNetCore;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using RentalFlow.Infrastructure.Data;
global using RentalFlow.Infrastructure.Repositories;
global using RentalFlow.Infrastructure.Services;
global using RentalFlow.Infrastructure.Settings;

global using RentalFlow.Application.UseCases.Commands.Applicant;
global using RentalFlow.Application.UseCases.Queries.Applicant;

global using RentalFlow.Application.Interfaces.Services;
global using RentalFlow.Application.Interfaces.Repositories;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;

global using RentalFlow.Application.Validators.RentalApplication;
global using RentalFlow.Application.Validators.Applicant;
global using RentalFlow.Application.Validators.Operator;
global using RentalFlow.Application.Validators.Property;