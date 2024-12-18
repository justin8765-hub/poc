CREATE PROCEDURE GetChangeDetails
    @TableID INT,
    @ChangeDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Columns NVARCHAR(MAX);       -- To store column names
    DECLARE @PrevStateSQL NVARCHAR(MAX); -- To fetch the previous state dynamically
    DECLARE @CurrStateSQL NVARCHAR(MAX); -- To fetch the current state dynamically
    DECLARE @JSON NVARCHAR(MAX);         -- To store the resulting JSON

    -- Step 1: Get all column names from the TableAudit (except TableID and ChangeDate)
    SELECT @Columns = STRING_AGG(COLUMN_NAME, ', ')
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TableAudit' AND COLUMN_NAME NOT IN ('TableID', 'ChangeDate');

    -- Step 2: Create SQL for fetching the previous state
    SET @PrevStateSQL = '
        SELECT ' + @Columns + '
        FROM TableAudit
        WHERE TableID = @TableID AND ChangeDate = (
            SELECT MAX(ChangeDate)
            FROM TableAudit
            WHERE TableID = @TableID AND ChangeDate < @ChangeDate
        )
    ';

    -- Step 3: Create SQL for fetching the current state
    SET @CurrStateSQL = '
        SELECT ' + @Columns + '
        FROM TableAudit
        WHERE TableID = @TableID AND ChangeDate = @ChangeDate
    ';

    -- Step 4: Compare the two states dynamically and generate JSON
    SET @JSON = '
        WITH PreviousState AS (
            ' + @PrevStateSQL + '
        ),
        CurrentState AS (
            ' + @CurrStateSQL + '
        )
        SELECT
            ''{ "TableId": '' + CAST(@TableID AS NVARCHAR) + '', "ChangeDate": "' + CONVERT(VARCHAR, @ChangeDate, 120) + '", "ChangeDetails": {' +
            STRING_AGG(
                ''"'' + COLUMN_NAME + '': {"Current": '' + ISNULL(CAST(cs.COLUMN_VALUE AS NVARCHAR), ''NULL'') + '', "Previous": '' + ISNULL(CAST(ps.COLUMN_VALUE AS NVARCHAR), ''NULL'') + ''}'', '', '') +
            ''} }''
        AS JSON_Result
        FROM INFORMATION_SCHEMA.COLUMNS col
        LEFT JOIN PreviousState ps ON ps.COLUMN_NAME = col.COLUMN_NAME
        LEFT JOIN CurrentState cs ON cs.COLUMN_NAME = col.COLUMN_NAME
        WHERE ISNULL(ps.VALUE, '') <> ISNULL(cs.VALUE);
    ';

    -- Step 5: Execute the dynamic SQL and return the JSON
    EXEC sp_executesql @JSON, N'@TableID INT, @ChangeDate DATETIME', @TableID, @ChangeDate;
END;
