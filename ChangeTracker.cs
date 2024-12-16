using System;
using System.Collections.Generic;
using System.Reflection;

public static class ChangeTracker
{
    public static List<ChangeLog> DetectChanges<T>(
        T? oldEntity,
        T? newEntity,
        string tableName,
        object primaryKeyValue,
        string changeType)
    {
        var changeLogs = new List<ChangeLog>();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var oldValue = oldEntity != null ? property.GetValue(oldEntity)?.ToString() : null;
            var newValue = newEntity != null ? property.GetValue(newEntity)?.ToString() : null;

            if (oldValue != newValue)
            {
                changeLogs.Add(new ChangeLog
                {
                    TableName = tableName,
                    PrimaryKey = primaryKeyValue.ToString() ?? "Unknown",
                    FieldName = property.Name,
                    OldValue = oldValue,
                    NewValue = newValue,
                    ChangeDate = DateTime.UtcNow,
                    ChangeType = changeType
                });
            }
        }

        return changeLogs;
    }
}
