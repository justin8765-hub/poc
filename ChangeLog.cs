CREATE PROCEDURE GenerateDifferenceJSON
AS
BEGIN
    -- Use a single query to compute differences and return JSON results
    SELECT 
        (SELECT 
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
