// See https://aka.ms/new-console-template for more information

using InMemoryDB;
using InMemoryDB.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string logPath = "db_wal.log";
        string backupPath = "db_backup.json";
        InMemoryDatabase db = new(logPath, backupPath);
        
        db.CreateTable("Users");
        db.Insert("Users", new Row { Data = new Dictionary<string, object> { { "Name", "Alice" }, { "Age", 30 } } });
        db.Insert("Users", new Row { Data = new Dictionary<string, object> { { "Name", "Bob" }, { "Age", 25 } } });
            
        Console.WriteLine("Database initialized and operations logged!");
            
        // Simulate failover delay
        await Task.Delay(5000);
        Console.WriteLine("Simulating a crash and recovery...");
        InMemoryDatabase recoveredDb = new(logPath, backupPath);
        Console.WriteLine("Database recovered from logs!");
    }
}