using ITask6.Game.Services;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.Connection;

public class GameHub : Hub
{
    private IMatchMakingService _matchMakingService;

    public GameHub(IMatchMakingService matchMakingService)
    {
        _matchMakingService = matchMakingService;
    }

    public async Task<bool> TryToConnect(string nickname)
    {
        bool isSuccessful = _matchMakingService.TryToAddPlayer(Context.ConnectionId, nickname);
        return isSuccessful;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _matchMakingService.RemovePlayer(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<List<Room>> GetRooms()
    {
        return _matchMakingService.GetRooms();
    }

    public async Task<Room?> CreateOwnRoom()
    {
        return _matchMakingService.CreateRoomAndJoin(Context.ConnectionId);
    }

    public async Task<Room?> JoinRoom(int roomId)
    {
        return _matchMakingService.JoinRoom(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom()
    {
        _matchMakingService.LeaveRoom(Context.ConnectionId);
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("message", Context.ConnectionId, message);
    }
}