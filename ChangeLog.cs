CREATE PROCEDURE dbo.GetUserFieldChangesAsJson
    @UserId INT,
    @AuditDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Fetch only the changes for the specific UserId and AuditDate
    SELECT 
        ua.AuditId,
        ua.AuditDate,
        ua.Operation,
        JSON_QUERY(
            CASE
                WHEN ua.Operation = 'UPDATE' THEN 
                    JSON_OBJECT(
                        'UserName', 
                        CASE WHEN ua.OldUserName <> ua.UserName THEN JSON_OBJECT('Old', ua.OldUserName, 'New', ua.UserName) ELSE NULL END,
                        'Email', 
                        CASE WHEN ua.OldEmail <> ua.Email THEN JSON_OBJECT('Old', ua.OldEmail, 'New', ua.Email) ELSE NULL END,
                        'ModifiedDate', 
                        CASE WHEN ua.OldModifiedDate <> ua.ModifiedDate THEN JSON_OBJECT('Old', CONVERT(VARCHAR, ua.OldModifiedDate, 120), 'New', CONVERT(VARCHAR, ua.ModifiedDate, 120)) ELSE NULL END
                    )
                WHEN ua.Operation = 'INSERT' THEN 
                    JSON_OBJECT('InsertedValues', JSON_OBJECT('UserName', ua.UserName, 'Email', ua.Email))
                WHEN ua.Operation = 'DELETE' THEN 
                    JSON_OBJECT('DeletedValues', JSON_OBJECT('UserName', ua.UserName, 'Email', ua.Email))
            END
        ) AS ChangeDetails
    FROM 
        dbo.UserAudit ua
    WHERE 
        ua.UserId = @UserId
        AND CAST(ua.AuditDate AS DATE) = CAST(@AuditDate AS DATE)
    ORDER BY 
        ua.AuditDate DESC
    FOR JSON PATH;
END;
