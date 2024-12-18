CREATE PROCEDURE GetChangeDetails
    @TableID INT,
    @ChangeDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate JSON of changes dynamically
    SELECT (
        '{ "TableId": ' + CAST(@TableID AS VARCHAR) + ', "ChangeDetails": {' +
        STRING_AGG(
            '\"' + col.COLUMN_NAME + '\": {\"Current\": \"' + ISNULL(CAST(cs.VALUE AS NVARCHAR), 'NULL') + '\", \"Previous\": \"' + ISNULL(CAST(ps.VALUE AS NVARCHAR), 'NULL') + '\"}', ', ') +
        '} }'
    ) AS JSON_Result
    FROM (
        -- Get the previous state
        SELECT * 
        FROM TableAudit 
        WHERE TableID = @TableID AND ChangeDate < @ChangeDate
        ORDER BY ChangeDate DESC
        OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY
    ) ps
    FULL OUTER JOIN (
        -- Get the current state
        SELECT * 
        FROM TableAudit 
        WHERE TableID = @TableID AND ChangeDate = @ChangeDate
    ) cs
    ON 1=1 -- Compare all columns dynamically
    CROSS APPLY (
        SELECT COLUMN_NAME, ps.VALUE AS PreviousValue, cs.VALUE AS CurrentValue
        FROM INFORMATION_SCHEMA.COLUMNS col
        WHERE col.TABLE_NAME = 'TableAudit'
    ) col
    WHERE ISNULL(ps.VALUE, 'NULL') <> ISNULL(cs.VALUE, 'NULL');
END
