CREATE PROCEDURE GetChangeDetails
    @TableID INT,
    @ChangeDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate JSON of changes between previous and current states
    SELECT (
        '{ "TableId": ' + CAST(@TableID AS VARCHAR) + ', "ChangeDetails": {' +
        STRING_AGG(
            '"' + COLUMN_NAME + '": {"Current": "' + ISNULL(CAST(CurrentValue AS NVARCHAR), 'NULL') + '", "Previous": "' + ISNULL(CAST(PreviousValue AS NVARCHAR), 'NULL') + '"}', ', ') +
        '} }'
    ) AS JSON_Result
    FROM (
        SELECT
            a.COLUMN_NAME,
            b.VALUE AS PreviousValue,
            c.VALUE AS CurrentValue
        FROM
            (SELECT * FROM TableAudit WHERE TableID = @TableID AND ChangeDate < @ChangeDate ORDER BY ChangeDate DESC LIMIT 1) b
        FULL OUTER JOIN
            (SELECT * FROM TableAudit WHERE TableID = @TableID AND ChangeDate = @ChangeDate) c
        ON b.COLUMN_NAME = c.COLUMN_NAME
    ) Changes
    WHERE ISNULL(PreviousValue, 'NULL') <> ISNULL(CurrentValue, 'NULL');

END
