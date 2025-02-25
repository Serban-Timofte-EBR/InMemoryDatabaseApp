namespace InMemoryDB.Models;

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Operation { get; set; }
    public string Table { get; set; }
    public Row Data { get; set; }
}