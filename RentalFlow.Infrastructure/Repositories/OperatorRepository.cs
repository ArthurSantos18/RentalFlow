using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Operator;
using RentalFlow.Domain.Entities.Operator;
using RentalFlow.Domain.Enums;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class OperatorRepository(AppDbContext context) : BaseRepository<OperatorEntity>(context), IOperatorRepository
{
    public async Task<PagedResult<OperatorEntity>> GetOperatorsAsync(GetOperatorRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Operators.AsNoTracking().AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyNamesFilter(query, request.Names);
        query = ApplyEmailsFilter(query, request.Emails);
        query = ApplyRoleFilter(query, request.Role);
        query = ApplyActiveFilter(query, request.IsActive);
        query = ApplyHasApplicationsFilter(query, request.HasApplications);
        query = ApplyApplicationIdsFilter(query, request.ApplicationIds);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<OperatorEntity>(results, total, page, pageSize);
    }

    private static IQueryable<OperatorEntity> ApplyIdsFilter(IQueryable<OperatorEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(o => ids.Contains(o.Id));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyNamesFilter(IQueryable<OperatorEntity> query, IEnumerable<string>? names)
    {
        if (names?.Any() == true)
        {
            var namesLower = names.Select(n => n.ToLower()).ToList();
            return query.Where(o => namesLower.Contains(o.Name.ToLower()));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyEmailsFilter(IQueryable<OperatorEntity> query, IEnumerable<string>? emails)
    {
        if (emails?.Any() == true)
        {
            var emailsLower = emails.Select(e => e.ToLower()).ToList();
            return query.Where(o => emailsLower.Contains(o.Email.ToLower()));
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyRoleFilter(IQueryable<OperatorEntity> query, OperatorRole? role)
    {
        if (role.HasValue && role != OperatorRole.None)
        {
            return query.Where(o => o.Role == role.Value);
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyActiveFilter(IQueryable<OperatorEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(o => o.IsActive == isActive.Value);
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyHasApplicationsFilter(IQueryable<OperatorEntity> query, bool? hasApplications)
    {
        if (hasApplications.HasValue)
        {
            return hasApplications.Value ? query.Where(o => o.Applications.Any()) : query.Where(o => !o.Applications.Any());
        }

        return query;
    }

    private static IQueryable<OperatorEntity> ApplyApplicationIdsFilter(IQueryable<OperatorEntity> query, IEnumerable<Guid>? applicationIds)
    {
        if (applicationIds?.Any() == true)
        {
            return query.Where(o =>o.Applications.Any(a => applicationIds.Contains(a.Id)));
        }

        return query;
    }
}
