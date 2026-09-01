using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Application.Requests.Applicant;
using RentalFlow.Domain.Entities.Applicant;
using RentalFlow.Domain.Patterns.PagedResult;
using RentalFlow.Infrastructure.Data;

namespace RentalFlow.Infrastructure.Repositories;

public sealed class ApplicantRepository(AppDbContext context) : BaseRepository<ApplicantEntity>(context), IApplicantRepository
{
    public async Task<ApplicantEntity?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Cpf == cpf, cancellationToken);
    }

    public async Task<PagedResult<ApplicantEntity>> GetApplicantsAsync(GetApplicantRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyFullNamesFilter(query, request.FullNames);
        query = ApplyCpfsFilter(query, request.Cpfs);
        query = ApplyEmailsFilter(query, request.Emails);
        query = ApplyPhonesFilter(query, request.Phones);
        query = ApplyMonthlyIncomeFilter(query, request.MinMonthlyIncome, request.MaxMonthlyIncome);
        query = ApplyIsActiveFilter(query, request.IsActive);
        query = ApplyHasApplicationsFilter(query, request.HasApplications);
        query = ApplyApplicationIdsFilter(query, request.ApplicationIds);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ApplicantEntity>(results, total, page, pageSize);
    }

    private static IQueryable<ApplicantEntity> ApplyIdsFilter(IQueryable<ApplicantEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(a => ids.Contains(a.Id));
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyFullNamesFilter(IQueryable<ApplicantEntity> query, IEnumerable<string>? fullNames)
    {
        if (fullNames?.Any() == true)
        {
            var namesLower = fullNames.Select(n => n.ToLower()).ToList();
            return query.Where(a => namesLower.Contains(a.FullName.ToLower()));
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyCpfsFilter(IQueryable<ApplicantEntity> query, IEnumerable<string>? cpfs)
    {
        if (cpfs?.Any() == true)
        {
            var cpfsClean = cpfs.Select(c => c.Replace(".", "").Replace("-", "")).ToList();
            return query.Where(a => cpfsClean.Contains(a.Cpf.Replace(".", "").Replace("-", "")));
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyEmailsFilter(IQueryable<ApplicantEntity> query, IEnumerable<string>? emails)
    {
        if (emails?.Any() == true)
        {
            var emailsLower = emails.Select(e => e.ToLower()).ToList();
            return query.Where(a => emailsLower.Contains(a.Email.ToLower()));
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyPhonesFilter(IQueryable<ApplicantEntity> query, IEnumerable<string>? phones)
    {
        if (phones?.Any() == true)
        {
            var phonesClean = phones.Select(p => p.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "")).ToList();
            return query.Where(a => phonesClean.Contains(a.Phone != null ? a.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : ""));
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyMonthlyIncomeFilter(IQueryable<ApplicantEntity> query, decimal? minIncome, decimal? maxIncome)
    {
        if (minIncome.HasValue)
        {
            query = query.Where(a => a.MonthlyIncome >= minIncome.Value);
        }
            
        if (maxIncome.HasValue)
        {
            query = query.Where(a => a.MonthlyIncome <= maxIncome.Value);
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyIsActiveFilter(IQueryable<ApplicantEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(a => a.IsActive == isActive.Value);
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyHasApplicationsFilter(IQueryable<ApplicantEntity> query, bool? hasApplications)
    {
        if (hasApplications.HasValue)
        {
            return hasApplications.Value ? query.Where(a => a.Applications.Any()) : query.Where(a => !a.Applications.Any());
        }

        return query;
    }

    private static IQueryable<ApplicantEntity> ApplyApplicationIdsFilter(IQueryable<ApplicantEntity> query, IEnumerable<Guid>? applicationIds)
    {
        if (applicationIds?.Any() == true)
        {
            return query.Where(a => a.Applications.Any(app => applicationIds.Contains(app.Id)));
        }

        return query;
    }
}
