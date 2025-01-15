SELECT 
    qs.sql_handle,
    qs.plan_handle,
    q.text AS QueryText,
    qs.execution_count,
    qs.total_elapsed_time,
    qs.last_execution_time
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) q
WHERE qs.last_execution_time >= DATEADD(MINUTE, -10, GETDATE()) -- last 10 minutes
AND EXISTS (
    SELECT 1
    FROM sys.dm_os_ring_buffers rb
    WHERE rb.ring_buffer_type = 'RING_BUFFER_EXCEPTION'
    AND CAST(rb.record_id AS INT) = qs.sql_handle
);
