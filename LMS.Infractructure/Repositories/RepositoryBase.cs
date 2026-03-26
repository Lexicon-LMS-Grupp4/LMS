using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infractructure.Repositories;

public abstract class RepositoryBase<T>(ApplicationDbContext context) : IRepositoryBase<T>, IInternalRepositoryBase<T> where T : EntityBase
{
    protected DbSet<T> DbSet { get; } = context.Set<T>();
    public async Task<T?> FindByIdAsync(int? id) => await DbSet.FindAsync(id);

    public async Task<T> FindByIdOrThrowAsync(int id, bool trackChanges)
    {
        var query = DbSet.AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        var entity = await query.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"{typeof(T).Name} with id {id} was not found.");
        }
        return entity;
    }

    public async Task<bool> ExistsAsync(int id) => await DbSet.AnyAsync(e => e.Id == id);

    /// <summary>
    /// Returns a queryable collection of all entities in the set.
    /// </summary>
    /// <remarks>When change tracking is disabled, the returned entities are not tracked by the context, which
    /// can improve performance for read-only operations.</remarks>
    /// <param name="trackChanges">true to enable change tracking for the returned entities; otherwise, false to disable tracking and improve query
    /// performance.</param>
    /// <returns>An IQueryable<T> representing all entities in the set. The query may be further composed before execution.</returns>
    public IQueryable<T> FindAll(bool trackChanges = false) =>
        !trackChanges ? DbSet.AsNoTracking() :
                        DbSet;

    /// <summary>
    /// Retrieves entities that satisfy the specified condition as a queryable collection.
    /// </summary>
    /// <remarks>When trackChanges is set to false, the returned entities are not tracked by the context,
    /// which can improve performance for read-only operations.</remarks>
    /// <param name="expression">An expression that defines the condition to filter the entities.</param>
    /// <param name="trackChanges">true to enable change tracking for the returned entities; otherwise, false to retrieve entities without
    /// tracking.</param>
    /// <returns>An IQueryable<T> containing entities that match the specified condition.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
        !trackChanges ? DbSet.Where(expression).AsNoTracking() :
                        DbSet.Where(expression);

    public void Create(T entity) => DbSet.Add(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);
}
