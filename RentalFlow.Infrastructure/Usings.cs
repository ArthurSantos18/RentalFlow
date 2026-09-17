global using System.Linq.Expressions;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;

global using BCrypt.Net;

global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.DependencyInjection;

global using Microsoft.IdentityModel.Tokens;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Infrastructure;

global using RentalFlow.Application.Interfaces.Repositories;
global using RentalFlow.Application.Interfaces.Services;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.Requests.Team;
global using RentalFlow.Application.Requests.User;

global using RentalFlow.Infrastructure.Data;
global using RentalFlow.Infrastructure.Settings;

global using RentalFlow.Domain.Patterns.PagedResult;
global using RentalFlow.Domain.Entities;
global using RentalFlow.Domain.Enums;