namespace RentalFlow.Infrastructure.Repositories;

public sealed class RentalApplicationRepository(AppDbContext context) : BaseRepository<RentalApplicationEntity>(context), IRentalApplicationRepository
{
    public async Task<int> CountByOperatorAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.CountAsync(ra => ra.OperatorId == id, cancellationToken);
    }

    public async Task<bool> PropertyHasApplicationsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(ra => ra.PropertyId == id, cancellationToken);
    }

    public async Task<bool> ApplicantHasApplicationsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(ra => ra.ApplicantId == id, cancellationToken);
    }

    public async Task<PagedResult<RentalApplicationEntity>> GetRentalApplicationsAsync(GetRentalApplicationRequest request, DataScope scope, CancellationToken cancellationToken)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(x => x.Applicant)
            .Include(x => x.Property)
            .Include(x => x.Operator)
            .AsQueryable();

        query = ApplyIdsFilter(query, request.Ids);
        query = ApplyProposalNumbersFilter(query, request.ProposalNumbers);
        query = ApplyApplicantIdsFilter(query, request.ApplicantIds);
        query = ApplyPropertyIdsFilter(query, request.PropertyIds);
        query = ApplyOperatorIdsFilter(query, request.OperatorIds);
        query = ApplyTeamIdsFilter(query, request.TeamIds);
        query = ApplyStatusFilter(query, request.Status);
        query = ApplyFinancedAmountFilter(query, request.MinFinancedAmount, request.MaxFinancedAmount);
        query = ApplyTotalAmountFilter(query, request.MinTotalAmount, request.MaxTotalAmount);
        query = ApplyCreatedAtFilter(query, request.MinCreatedAt, request.MaxCreatedAt);
        query = ApplyContractDateFilter(query, request.MinContractDate, request.MaxContractDate);
        query = ApplyIsActiveFilter(query, request.IsActive);
        query = ApplyDataScope(query, scope);

        var page = request.PageFilter.Page > 0 ? request.PageFilter.Page : 1;
        var pageSize = request.PageFilter.PageSize > 0 ? request.PageFilter.PageSize : 60;

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<RentalApplicationEntity>(results, total, page, pageSize);
    }

    private static IQueryable<RentalApplicationEntity> ApplyIdsFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<Guid>? ids)
    {
        if (ids?.Any() == true)
        {
            return query.Where(ra => ids.Contains(ra.Id));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyProposalNumbersFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<string>? proposalNumbers)
    {
        if (proposalNumbers?.Any() == true)
        {
            return query.Where(ra => proposalNumbers.Contains(ra.ProposalNumber));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyApplicantIdsFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<Guid>? applicantIds)
    {
        if (applicantIds?.Any() == true)
        {
            return query.Where(ra => applicantIds.Contains(ra.ApplicantId));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyPropertyIdsFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<Guid>? propertyIds)
    {
        if (propertyIds?.Any() == true)
        {
            return query.Where(ra => propertyIds.Contains(ra.PropertyId));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyOperatorIdsFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<Guid>? operatorIds)
    {
        if (operatorIds?.Any() == true)
        {
            return query.Where(ra => operatorIds.Contains(ra.OperatorId));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyTeamIdsFilter(IQueryable<RentalApplicationEntity> query, IEnumerable<Guid>? teamIds)
    {
        if (teamIds?.Any() == true)
        {
            query = query.Where(ra => ra.Operator != null && teamIds.Contains(ra.Operator.TeamId));
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyStatusFilter(IQueryable<RentalApplicationEntity> query, RentalStatus? status)
    {
        if (status.HasValue && status != RentalStatus.None)
        {
            return query.Where(ra => ra.Status == status.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyFinancedAmountFilter(IQueryable<RentalApplicationEntity> query, decimal? min, decimal? max)
    {
        if (min.HasValue)
        {
            query = query.Where(ra => ra.FinancedAmount >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(ra => ra.FinancedAmount <= max.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyTotalAmountFilter(IQueryable<RentalApplicationEntity> query, decimal? min, decimal? max)
    {
        if (min.HasValue)
        {
            query = query.Where(ra => ra.TotalAmount >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(ra => ra.TotalAmount <= max.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyCreatedAtFilter(IQueryable<RentalApplicationEntity> query, DateTime? min, DateTime? max)
    {
        if (min.HasValue)
        {
            query = query.Where(ra => ra.CreatedAt >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(ra => ra.CreatedAt <= max.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyContractDateFilter(IQueryable<RentalApplicationEntity> query, DateTime? min, DateTime? max)
    {
        if (min.HasValue)
        {
            query = query.Where(ra => ra.ContractDate >= min.Value);
        }

        if (max.HasValue)
        {
            query = query.Where(ra => ra.ContractDate <= max.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyIsActiveFilter(IQueryable<RentalApplicationEntity> query, bool? isActive)
    {
        if (isActive.HasValue)
        {
            return query.Where(ra => ra.IsActive == isActive.Value);
        }

        return query;
    }

    private static IQueryable<RentalApplicationEntity> ApplyDataScope(IQueryable<RentalApplicationEntity> query, DataScope scope)
    {
        if (scope.IsGlobal)
        {
            return query;
        }

        if (scope.OperatorIds?.Any() == true)
        {
            query = query.Where(ra => scope.OperatorIds.Contains(ra.OperatorId));
        }

        if (scope.TeamIds?.Any() == true)
        {
            query = query.Where(ra => ra.Operator != null && scope.TeamIds.Contains(ra.Operator.TeamId));
        }

        if (scope.OperatorIds is null && scope.TeamIds is null)
        {
            query = query.Where(_ => false);
        }

        return query;
    }
}