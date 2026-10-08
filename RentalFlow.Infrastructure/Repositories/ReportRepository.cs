namespace RentalFlow.Infrastructure.Repositories;

public sealed class ReportRepository(AppDbContext _context) : IReportRepository
{
    public async Task<ApplicantAggregate> GetApplicantAggregateAsync(CancellationToken cancellationToken)
    {
        var total = await _context.Applicants.CountAsync(cancellationToken);
        var active = await _context.Applicants.CountAsync(a => a.IsActive, cancellationToken);

        return new ApplicantAggregate
        {
            Total = total,
            Active = active
        };
    }

    public async Task<PropertyAggregate> GetPropertyAggregateAsync(CancellationToken cancellationToken)
    {
        var total = await _context.Properties.CountAsync(cancellationToken);
        var available = await _context.Properties.CountAsync(p => p.IsAvailable, cancellationToken);
        var active = await _context.Properties.CountAsync(p => p.IsActive, cancellationToken);

        return new PropertyAggregate
        {
            Total = total,
            Available = available,
            Active = active
        };
    }

    public async Task<RentalApplicationAggregate> GetRentalApplicationAggregateAsync(CancellationToken cancellationToken)
    {
        var total = await _context.RentalApplications.CountAsync(cancellationToken);

        var byStatus = await _context.RentalApplications
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        var totalFinancedAmount = await _context.RentalApplications.SumAsync(r => (decimal?)r.FinancedAmount, cancellationToken) ?? 0;
        var totalAmount = await _context.RentalApplications.SumAsync(r => (decimal?)r.TotalAmount, cancellationToken) ?? 0;

        return new RentalApplicationAggregate
        {
            Total = total,
            Draft = byStatus.GetValueOrDefault(RentalStatus.Draft),
            Pending = byStatus.GetValueOrDefault(RentalStatus.Pending),
            Approved = byStatus.GetValueOrDefault(RentalStatus.Approved),
            Rejected = byStatus.GetValueOrDefault(RentalStatus.Rejected),
            TotalFinancedAmount = totalFinancedAmount,
            TotalAmount = totalAmount
        };
    }

    public async Task<OperatorAggregate> GetOperatorAggregateAsync(CancellationToken cancellationToken)
    {
        var total = await _context.Operators.CountAsync(cancellationToken);
        var active = await _context.Operators.CountAsync(o => o.IsActive, cancellationToken);

        var byRole = await _context.Operators
            .GroupBy(o => o.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Role, x => x.Count, cancellationToken);

        return new OperatorAggregate
        {
            Total = total,
            Active = active,
            Broker = byRole.GetValueOrDefault(OperatorRole.Broker),
            Manager = byRole.GetValueOrDefault(OperatorRole.Manager),
            Administrator = byRole.GetValueOrDefault(OperatorRole.Administrator)
        };
    }

    public async Task<TeamAggregate> GetTeamAggregateAsync(CancellationToken cancellationToken)
    {
        var total = await _context.Teams.CountAsync(cancellationToken);
        var active = await _context.Teams.CountAsync(t => t.IsActive, cancellationToken);

        return new TeamAggregate
        {
            Total = total,
            Active = active
        };
    }

    public async Task<IReadOnlyList<TopPropertyAggregate>> GetTopPropertyAggregateAsync(GetTopPropertiesRequest request, CancellationToken cancellationToken)
    {
        var query = _context.RentalApplications
            .AsNoTracking()
            .AsQueryable();

        query = ApplyDateFilter(query, request.From, request.To);

        return await query
            .GroupBy(r => r.PropertyId)
            .Select(g => new TopPropertyAggregate
            {
                PropertyId = g.Key,
                Total = g.Count(),
                Approved = g.Count(x => x.Status == RentalStatus.Approved),
                Pending = g.Count(x => x.Status == RentalStatus.Pending),
                Rejected = g.Count(x => x.Status == RentalStatus.Rejected)
            })
            .OrderByDescending(x => x.Total)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TopOperatorAggregate>> GetTopOperatorAggregateAsync(GetTopOperatorsRequest request, CancellationToken cancellationToken)
    {
        var query = _context.RentalApplications
            .AsNoTracking()
            .AsQueryable();

        query = ApplyDateFilter(query, request.From, request.To);

        return await query
            .GroupBy(r => r.OperatorId)
            .Select(g => new TopOperatorAggregate
            {
                OperatorId = g.Key,
                Total = g.Count(),
                Approved = g.Count(x => x.Status == RentalStatus.Approved),
                Pending = g.Count(x => x.Status == RentalStatus.Pending),
                Rejected = g.Count(x => x.Status == RentalStatus.Rejected),
                TotalAmount = g.Sum(x => x.TotalAmount)
            })
            .OrderByDescending(x => x.Total)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationsByPeriodAggregate>> GetApplicationsByPeriodAsync(GetApplicationsByPeriodRequest request, CancellationToken cancellationToken)
    {
        var referenceDate = Helpers.PeriodHelper.NormalizeReferenceDate(request.From, request.GroupBy);

        var query = _context.RentalApplications
            .AsNoTracking()
            .Where(r => r.CreatedAt >= request.From && r.CreatedAt <= request.To);

        var periodKey = Helpers.PeriodHelper.GetGroupKey(referenceDate, request.GroupBy);

        var grouped = await query
            .GroupBy(periodKey)
            .Select(g => new
            {
                PeriodIndex = g.Key,
                Total = g.Count(),
                Approved = g.Count(x => x.Status == RentalStatus.Approved),
                Pending = g.Count(x => x.Status == RentalStatus.Pending),
                Rejected = g.Count(x => x.Status == RentalStatus.Rejected),
                TotalAmount = g.Sum(x => x.TotalAmount),
                TotalFinanced = g.Sum(x => x.FinancedAmount)
            })
            .OrderBy(x => x.PeriodIndex)
            .ToListAsync(cancellationToken);

        return [.. grouped
            .Select(g => new ApplicationsByPeriodAggregate
            {
                PeriodStart = Helpers.PeriodHelper.GetPeriodStart(referenceDate, request.GroupBy, g.PeriodIndex),
                Total = g.Total,
                Approved = g.Approved,
                Pending = g.Pending,
                Rejected = g.Rejected,
                TotalAmount = g.TotalAmount,
                TotalFinancedAmount = g.TotalFinanced
            })];
    }

    public async Task<ConversionRateAggregate> GetConversionRateAggregateAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var query = _context.RentalApplications
            .AsNoTracking()
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to);

        var byStatus = await query
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        var total = byStatus.Values.Sum();

        return new ConversionRateAggregate
        {
            Total = total,
            Draft = byStatus.GetValueOrDefault(RentalStatus.Draft),
            Pending = byStatus.GetValueOrDefault(RentalStatus.Pending),
            Approved = byStatus.GetValueOrDefault(RentalStatus.Approved),
            Rejected = byStatus.GetValueOrDefault(RentalStatus.Rejected)
        };
    }

    private static IQueryable<RentalApplicationEntity> ApplyDateFilter(IQueryable<RentalApplicationEntity> query, DateTime? from, DateTime? to)
    {
        if (from.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= to.Value);
        }

        return query;
    }
}