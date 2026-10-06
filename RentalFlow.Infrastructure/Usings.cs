global using BCrypt.Net;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Migrations;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

global using Microsoft.IdentityModel.Tokens;

global using RentalFlow.Application.Interfaces.Repositories;
global using RentalFlow.Application.Interfaces.Services;

global using RentalFlow.Application.Models;

global using RentalFlow.Application.Requests.Applicant;
global using RentalFlow.Application.Requests.AuditLog;
global using RentalFlow.Application.Requests.Operator;
global using RentalFlow.Application.Requests.Property;
global using RentalFlow.Application.Requests.RentalApplication;
global using RentalFlow.Application.Requests.Team;
global using RentalFlow.Application.Requests.User;

global using RentalFlow.Domain.Attributes;
global using RentalFlow.Domain.Entities;
global using RentalFlow.Domain.Enums;

global using RentalFlow.Domain.Patterns.PagedResult;

global using RentalFlow.Infrastructure.Auditing;
global using RentalFlow.Infrastructure.Data;
global using RentalFlow.Infrastructure.Helpers;
global using RentalFlow.Infrastructure.Repositories;
global using RentalFlow.Infrastructure.Services;
global using RentalFlow.Infrastructure.Settings;

global using System.IdentityModel.Tokens.Jwt;

global using System.Linq.Expressions;

global using System.Security.Claims;
global using System.Security.Cryptography;

global using System.Globalization;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text;
