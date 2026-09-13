namespace RentalFlow.Domain.Entities;

public abstract class BaseEntity<TEntity> where TEntity : BaseEntity<TEntity>
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public bool IsActive { get; protected set; } = true;
    public bool IsDeleted { get; protected set; } = false;
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    public TEntity SetId(Guid id)
    {
        Id = id;
        return (TEntity)this;
    }

    public TEntity SetIsActive(bool isActive)
    {
        IsActive = isActive;
        return (TEntity)this;
    }

    public TEntity SetIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
        DeletedAt = isDeleted ? DateTime.UtcNow : null;
        return (TEntity)this;
    }

    public TEntity SetCreatedAt(DateTime createdAt)
    {
        CreatedAt = createdAt;
        return (TEntity)this;
    }

    public TEntity SetUpdatedAt(DateTime? updatedAt)
    {
        UpdatedAt = updatedAt;
        return (TEntity)this;
    }

    public TEntity MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        return (TEntity)this;
    }

    public TEntity Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        return (TEntity)this;
    }

    public TEntity Touch()
    {
        UpdatedAt = DateTime.UtcNow;
        return (TEntity)this;
    }
}

