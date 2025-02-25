using System.Collections.Concurrent;
using System.Text.Json;
using InMemoryDB.Models;
using InMemoryDB.Storage;

namespace InMemoryDB;

public class InMemoryDatabase
{
    private readonly ConcurrentDictionary<string, Table> _tables = new();
    private readonly WriteAheadLog _wal;
    private readonly string _backupFilePath;

    public InMemoryDatabase(string logFilePath, string backupFilePath)
    {
        _wal = new WriteAheadLog(logFilePath);
        _backupFilePath = backupFilePath;

        // Load from WAL logs or backup file
        if (File.Exists(_backupFilePath))
        {
            RecoverFromBackup();
        }
        else
        {
            RecoverFromLog();
        }
    }

    public void CreateTable(string tableName)
    {
        _tables.TryAdd(tableName, new Table(tableName));
    }

    public void Insert(string tableName, Row row)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            table.Rows[row.Id] = row;
            _wal.LogOperation("INSERT", tableName, row);
        }
    }
    
    private void RecoverFromBackup()
    {
        try
        {
            string json = File.ReadAllText(_backupFilePath);
            var restoredTables = JsonSerializer.Deserialize<ConcurrentDictionary<string, Table>>(json);
            if (restoredTables != null)
            {
                _tables.Clear();
                foreach (var kvp in restoredTables)
                {
                    _tables[kvp.Key] = kvp.Value;
                }
                Console.WriteLine("Database successfully restored from backup file.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading backup: {ex.Message}");
        }
    }

    public void RecoverFromLog()
    {
        foreach (var log in _wal.ReadLogs())
        {
            try
            {
                var logEntry = JsonSerializer.Deserialize<LogEntry>(log);
                if (logEntry != null && logEntry.Operation == "INSERT")
                {
                    CreateTable(logEntry.Table);
                    Insert(logEntry.Table, logEntry.Data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing log entry: {ex.Message}");
            }
        }
    }

    private void SaveDatabaseToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_tables);
            File.WriteAllText(_backupFilePath, json);
            Console.WriteLine($"Wrote backup to: {_backupFilePath}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro saving database: {e.Message}");
            throw;
        }
    }

    ~InMemoryDatabase()
    {
        SaveDatabaseToFile();
    }
}