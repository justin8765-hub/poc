-- Step 1: Create the Audit Table
CREATE TABLE AuditTable (
    AuditID INT IDENTITY(1,1) PRIMARY KEY,
    Operation NVARCHAR(10) NOT NULL, -- INSERT, UPDATE, DELETE
    TableName NVARCHAR(50) NOT NULL, -- Name of the audited table
    PrimaryKey NVARCHAR(MAX) NOT NULL, -- JSON representation of the primary key
    OldValues NVARCHAR(MAX) NULL, -- JSON representation of old values
    NewValues NVARCHAR(MAX) NULL, -- JSON representation of new values
    Timestamp DATETIME DEFAULT GETDATE(), -- Date and time of the operation
    [User] NVARCHAR(50) NOT NULL -- User who performed the operation
);

-- Step 2: Create the Insert Trigger
CREATE TRIGGER trg_Audit_Insert
ON Employees
AFTER INSERT
AS
BEGIN
    INSERT INTO AuditTable (Operation, TableName, PrimaryKey, NewValues, Timestamp, [User])
    SELECT
        'INSERT',
        'Employees',
        (SELECT CONCAT('{"EmployeeID":', EmployeeID, '}') FROM INSERTED),
        (SELECT * FROM INSERTED FOR JSON PATH),
        GETDATE(),
        SYSTEM_USER
    FROM INSERTED;
END;
GO

-- Step 3: Create the Update Trigger
CREATE TRIGGER trg_Audit_Update
ON Employees
AFTER UPDATE
AS
BEGIN
    INSERT INTO AuditTable (Operation, TableName, PrimaryKey, OldValues, NewValues, Timestamp, [User])
    SELECT
        'UPDATE',
        'Employees',
        (SELECT CONCAT('{"EmployeeID":', i.EmployeeID, '}') FROM INSERTED i),
        (SELECT * FROM DELETED FOR JSON PATH),
        (SELECT * FROM INSERTED FOR JSON PATH),
        GETDATE(),
        SYSTEM_USER
    FROM INSERTED
    INNER JOIN DELETED ON INSERTED.EmployeeID = DELETED.EmployeeID;
END;
GO

-- Step 4: Create the Delete Trigger
CREATE TRIGGER trg_Audit_Delete
ON Employees
AFTER DELETE
AS
BEGIN
    INSERT INTO AuditTable (Operation, TableName, PrimaryKey, OldValues, Timestamp, [User])
    SELECT
        'DELETE',
        'Employees',
        (SELECT CONCAT('{"EmployeeID":', d.EmployeeID, '}') FROM DELETED d),
        (SELECT * FROM DELETED FOR JSON PATH),
        GETDATE(),
        SYSTEM_USER
    FROM DELETED;
END;
GO

-- Verify the triggers and table creation
PRINT 'Audit table and triggers have been created successfully.'

    CREATE PROCEDURE GenerateDifferenceJSON
AS
BEGIN
    -- Use a single query to compute differences and return JSON results
    SELECT 
        (SELECT 
            t.Id AS [Id],
            t.Date AS [Date],
            JSONData1.[key] AS [Field], 
            JSONData1.value AS [Instance1Value], 
            JSONData2.value AS [Instance2Value]
         FROM OPENJSON(t.Instance1) AS JSONData1
         FULL OUTER JOIN OPENJSON(t.Instance2) AS JSONData2
         ON JSONData1.[key] = JSONData2.[key]
         WHERE ISNULL(JSONData1.value, '') != ISNULL(JSONData2.value, '')
         FOR JSON PATH) AS DifferenceJSON
    FROM YourTable AS t; -- Replace 'YourTable' with your actual table name
END

