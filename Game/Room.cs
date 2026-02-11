using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game;

public class Room(int capacity, Hub hub)
{
    [JsonPropertyName("id")]
    public int Id { get; } = _nextId++;
    
    [JsonPropertyName("players")]
    public Dictionary<string, string> Players { get; } = new();
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; } = capacity;
    
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable => Players.Count < Capacity;

    protected Hub Hub = hub;
    
    private static int _nextId = 0;

    public void AddPlayer(string id, string nickname)
    {
        Players[id] = nickname;
        OnPlayerAdded(id);
    }
    
    public void RemovePlayer(string id)
    {
        Players.Remove(id);
        OnPlayerRemoved(id);
    }

    public void StartGame()
    {
        if (!CanStartGame()) return;
        OnGameStarted();
    }

    public bool ContainsPlayer(string id)
    {
        return Players.ContainsKey(id);
    }

    public bool IsEmpty()
    {
        return Players.Count == 0;
    }
    
    public virtual bool CanStartGame()
    {
        return true;
    }
    public virtual void PlayerAction(string id, string type, string action){}
    protected virtual void OnPlayerAdded(string id){}
    protected virtual void OnPlayerRemoved(string id){}
    protected virtual void OnGameStarted(){}
}