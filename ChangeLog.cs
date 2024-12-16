public class ChangeLog
{
    public int ChangeLogId { get; set; }
    public string TableName { get; set; }
    public string PrimaryKey { get; set; }
    public string FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangeDate { get; set; }
    public string ChangeType { get; set; }
}
