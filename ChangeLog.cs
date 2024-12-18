CREATE PROCEDURE GetChangeDetails
    @TableID INT,
    @ChangeDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Temporary tables to store previous and current states
    DECLARE @PreviousState TABLE (ColumnName NVARCHAR(MAX), Value NVARCHAR(MAX));
    DECLARE @CurrentState TABLE (ColumnName NVARCHAR(MAX), Value NVARCHAR(MAX));

    -- Extract previous state
    INSERT INTO @PreviousState (ColumnName, Value)
    SELECT COLUMN_NAME, CAST(COLUMN_VALUE AS NVARCHAR(MAX))
    FROM INFORMATION_SCHEMA.COLUMNS AS cols
    CROSS APPLY (
        SELECT cols.COLUMN_NAME, COLUMN_VALUE = CAST(audit.[cols.COLUMN_NAME] AS NVARCHAR(MAX))
        FROM TableAudit audit
        WHERE audit.TableID = @TableID AND audit.ChangeDate = (
            SELECT MAX(ChangeDate)
            FROM TableAudit
            WHERE TableID = @TableID AND ChangeDate < @ChangeDate
        )
    ) AS data;

    -- Extract current state
    INSERT INTO @CurrentState (ColumnName, Value)
    SELECT COLUMN_NAME, CAST(COLUMN_VALUE AS NVARCHAR(MAX))
    FROM INFORMATION_SCHEMA.COLUMNS AS cols
    CROSS APPLY (
        SELECT cols.COLUMN_NAME, COLUMN_VALUE = CAST(audit.[cols.COLUMN_NAME] AS NVARCHAR(MAX))
        FROM TableAudit audit
        WHERE audit.TableID = @TableID AND audit.ChangeDate = @ChangeDate
    ) AS data;

    -- Generate JSON of changes
    SELECT (
        '{ "TableId": ' + CAST(@TableID AS VARCHAR) + ', "ChangeDate": "' + CONVERT(VARCHAR, @ChangeDate, 120) + '", "ChangeDetails": {' +
        STRING_AGG(
            '\"' + ColumnName + '\": {\"Current\": \"' + ISNULL(Current.Value, 'NULL') + '\", \"Previous\": \"' + ISNULL(Previous.Value, 'NULL') + '\"}', ', ') +
        '} }'
    ) AS JSON_Result
    FROM @PreviousState Previous
    FULL OUTER JOIN @CurrentState Current
        ON Previous.ColumnName = Current.ColumnName
    WHERE ISNULL(Previous.Value, 'NULL') <> ISNULL(Current.Value, 'NULL');
END;
