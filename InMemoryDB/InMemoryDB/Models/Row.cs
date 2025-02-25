namespace InMemoryDB.Models;

public class Row
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Dictionary<string, object> Data { get; set; } = new();
}