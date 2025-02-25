using System.Collections.Concurrent;

namespace InMemoryDB.Models;

public class Table
{
    public string Name { get; }
    public ConcurrentDictionary<Guid, Row> Rows { get; } = new();

    public Table(string name)
    {
        Name = name;
    }
}