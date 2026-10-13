global using FluentAssertions;

global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.RateLimiting;

global using Microsoft.EntityFrameworkCore;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;

global using RentalFlow.API;

global using RentalFlow.Domain.Patterns.PagedResult;
global using RentalFlow.Domain.Enums;
global using RentalFlow.Domain.ValueObject;

global using RentalFlow.Application.Requests.Auth;
global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.Requests.Team;

global using RentalFlow.Application.Responses;

global using RentalFlow.Application.Responses.Report.ApplicationByPeriod;
global using RentalFlow.Application.Responses.Report.ConversionRate;
global using RentalFlow.Application.Responses.Report.Dashboard;
global using RentalFlow.Application.Responses.Report.TopOperator;
global using RentalFlow.Application.Responses.Report.TopProperty;

global using RentalFlow.Infrastructure.Auditing;
global using RentalFlow.Infrastructure.Data;

global using RentalFlow.IntegrationTests.Base;
global using RentalFlow.IntegrationTests.Fixtures;
global using RentalFlow.IntegrationTests.Helpers;

global using Testcontainers.MsSql;

global using System.Threading.RateLimiting;

global using System.Net.Http.Headers;
global using System.Net.Http.Json;

global using System.Net;