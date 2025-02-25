using System.Text.Json;
using InMemoryDB.Models;

namespace InMemoryDB.Storage;

public class WriteAheadLog
{
    private readonly string _logFilePath;
    private readonly object _lock = new();

    public WriteAheadLog(string logFilePath)
    {
        _logFilePath = logFilePath;
        if (!File.Exists(_logFilePath))
        {
            File.Create(_logFilePath).Dispose();
        }
    }

    public void LogOperation(string operation, string tableName, Row row)
    {
        lock (_lock)
        {
            var logEntry = new { Timestamp = DateTime.UtcNow, Operation = operation, Table = tableName, Data = row };
            File.AppendAllText(_logFilePath, JsonSerializer.Serialize(logEntry) + "\n");
        }
    }

    public IEnumerable<string> ReadLogs()
    {
        lock (_lock)
        {
            return File.ReadAllLines(_logFilePath);
        }
    }
}