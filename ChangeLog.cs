SELECT 
    r.session_id,
    r.status,
    r.command,
    t.text AS QueryText,
    s.login_name,
    r.start_time,
    r.error_number
FROM sys.dm_exec_requests r
JOIN sys.dm_exec_sessions s ON r.session_id = s.session_id
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) t
WHERE s.login_name = 'YourUserName' -- Replace with the specific user
AND r.error_number IS NOT NULL; -- Queries that encountered errors
