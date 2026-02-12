using System.Text.Json.Serialization;
using ITask6.Game.Connection;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game;

public class Room(int capacity, IHubContext<GameHub> hubContext)
{
    [JsonPropertyName("id")]
    public int Id { get; } = _nextId++;
    
    [JsonPropertyName("players")]
    public Dictionary<string, string> Players { get; } = new();
    
    [JsonPropertyName("capacity")]
    public int Capacity { get; } = capacity;
    
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable => Players.Count < Capacity;

    private readonly IHubContext<GameHub> _hubContext = hubContext;
    
    private static int _nextId = 0;

    public async Task AddPlayer(string id, string nickname)
    {
        Players[id] = nickname;
        await OnPlayerAdded(id);
    }
    
    public async Task RemovePlayer(string id)
    {
        Players.Remove(id);
        await OnPlayerRemoved(id);
    }
    
    public async Task PlayerAction(string id, string type, string action)
    {
        await OnPlayerAction(id, type, action);
    }
    
    protected  async Task SendDataToAllPlayers(string method, object data)
    {
        foreach (string id in Players.Keys.ToArray())
        {
            await SendDataToPlayer(id, method, data);
        }
    }
    
    protected  async Task SendDataToPlayer(string id, string method, object data)
    {
        await _hubContext.Clients.User(id).SendAsync(method, data);
    }

    public bool ContainsPlayer(string id)
    {
        return Players.ContainsKey(id);
    }

    public bool IsEmpty()
    {
        return Players.Count == 0;
    }

    protected virtual Task OnPlayerAction(string id, string type, string action)
    {
        return Task.CompletedTask;
    }
    protected virtual Task OnPlayerAdded(string id)
    {
        return Task.CompletedTask;
    }
    protected virtual Task OnPlayerRemoved(string id)
    {
        return Task.CompletedTask;
    }
}