CREATE PROCEDURE dbo.GenerateAuditQuery
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @columns NVARCHAR(MAX);
    DECLARE @sql NVARCHAR(MAX);

    -- Dynamically retrieve columns from the Test table
    SELECT @columns = STRING_AGG(QUOTENAME(COLUMN_NAME), ',')
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Test' AND TABLE_SCHEMA = 'dbo';

    -- Build the dynamic SQL query
    SET @sql = N'SELECT Id, Action, AuditId, ' +
               STUFF((
                   SELECT ', ' + QUOTENAME(col.value) + N' AS UpdatedValue_' + QUOTENAME(col.value) + ',
                                 CASE 
                                     WHEN Action = ''DELETE'' THEN NULL 
                                     ELSE LAG(' + QUOTENAME(col.value) + N') OVER (PARTITION BY Id ORDER BY AuditId ASC) 
                                 END AS Value_' + QUOTENAME(col.value)
                   FROM STRING_SPLIT(@columns, ',') AS col
                   FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') +
               N' FROM dbo.Audit';

    -- Print the generated SQL for debugging (optional, can be removed in production)
    PRINT @sql;

    -- Execute the dynamically generated SQL
    EXEC sp_executesql @sql;
END;
GO
