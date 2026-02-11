using System.Text.Json.Serialization;

namespace ITask6.Game;

public class Room(int capacity)
{
    [JsonPropertyName("id")]
    public int Id { get; } = _nextId++;
    
    [JsonPropertyName("players")]
    public Dictionary<string, string> Players { get; } = new();
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; } = capacity;
    
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable => Players.Count < Capacity;
    private static int _nextId = 0;

    public void AddPlayer(string id, string nickname)
    {
        Players[id] = nickname;
    }
    
    public void RemovePlayer(string id)
    {
        Players.Remove(id);
    }

    public bool ContainsPlayer(string id)
    {
        return Players.ContainsKey(id);
    }

    public bool IsEmpty()
    {
        return Players.Count == 0;
    }
}