global using FluentValidation;

global using LiteBus.Commands.Abstractions;
global using LiteBus.Queries.Abstractions;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.RateLimiting;

global using Microsoft.IdentityModel.Tokens;

global using Microsoft.OpenApi;

global using RentalFlow.API.Constants;
global using RentalFlow.API.Extensions;
global using RentalFlow.API.Filters;
global using RentalFlow.API.Handlers;
global using RentalFlow.API.Helpers;
global using RentalFlow.API.Middlewares;
global using RentalFlow.API.Services;

global using RentalFlow.Application.Extensions;

global using RentalFlow.Application.Interfaces.Services;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.Auth;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.Requests.Team;

global using RentalFlow.Application.UseCases.Commands.Applicant;
global using RentalFlow.Application.UseCases.Commands.Auth;
global using RentalFlow.Application.UseCases.Commands.Operator;
global using RentalFlow.Application.UseCases.Commands.Property;
global using RentalFlow.Application.UseCases.Commands.RentalApplication;
global using RentalFlow.Application.UseCases.Commands.Team;

global using RentalFlow.Application.UseCases.Queries.Applicant;
global using RentalFlow.Application.UseCases.Queries.Operator;
global using RentalFlow.Application.UseCases.Queries.Property;
global using RentalFlow.Application.UseCases.Queries.RentalApplication;
global using RentalFlow.Application.UseCases.Queries.Team;

global using RentalFlow.Crosscutting.Extensions;

global using RentalFlow.Domain.Enums;
global using RentalFlow.Domain.Patterns.Result;

global using RentalFlow.Infrastructure.Data;
global using RentalFlow.Infrastructure.Extensions;

global using Scalar.AspNetCore;

global using Serilog;
global using Serilog.Context;

global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Diagnostics.HealthChecks;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Threading.RateLimiting;
global using System.Text;
