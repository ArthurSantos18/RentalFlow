using Microsoft.EntityFrameworkCore;
using RentalFlow.Application.Interfaces.Repositories;
using RentalFlow.Domain.Entities;
using RentalFlow.Infrastructure.Data;
using System.Linq.Expressions;

namespace RentalFlow.Infrastructure.Repositories;

public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : BaseEntity<T>
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
 return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
