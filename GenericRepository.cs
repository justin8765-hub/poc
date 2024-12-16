using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> where T : class
{
    private readonly YourDbContext _context;

    public GenericRepository(YourDbContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(T entity, string tableName, string primaryKeyName)
    {
        var dbSet = _context.Set<T>();
        var primaryKeyValue = GetPrimaryKeyValue(entity, primaryKeyName);
        var existingEntity = await dbSet.FindAsync(primaryKeyValue);

        if (existingEntity == null)
        {
            await dbSet.AddAsync(entity);
            var changeLogs = ChangeTracker.DetectChanges(null, entity, tableName, primaryKeyValue, "Insert");
            await _context.ChangeLogs.AddRangeAsync(changeLogs);
        }
        else
        {
            var clonedEntity = CloneEntity(existingEntity);
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            var changeLogs = ChangeTracker.DetectChanges(clonedEntity, entity, tableName, primaryKeyValue, "Update");
            await _context.ChangeLogs.AddRangeAsync(changeLogs);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(object primaryKeyValue, string tableName, string primaryKeyName)
    {
        var dbSet = _context.Set<T>();
        var entity = await dbSet.FindAsync(primaryKeyValue);

        if (entity != null)
        {
            dbSet.Remove(entity);

            var changeLogs = ChangeTracker.DetectChanges(entity, null, tableName, primaryKeyValue, "Delete");
            await _context.ChangeLogs.AddRangeAsync(changeLogs);
        }

        await _context.SaveChangesAsync();
    }

    private object GetPrimaryKeyValue(T entity, string primaryKeyName)
    {
        var property = typeof(T).GetProperty(primaryKeyName);
        if (property == null)
        {
            throw new InvalidOperationException($"Primary key '{primaryKeyName}' not found on entity '{typeof(T).Name}'.");
        }

        return property.GetValue(entity) ?? throw new InvalidOperationException($"Primary key value for '{primaryKeyName}' cannot be null.");
    }

    private T CloneEntity(T entity)
    {
        var json = JsonSerializer.Serialize(entity);
        return JsonSerializer.Deserialize<T>(json);
    }
}
