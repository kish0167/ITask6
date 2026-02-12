using ITask6.Game.Connection;
using Microsoft.AspNetCore.SignalR;

namespace ITask6.Game.MatchMaking;

public interface IMatchMakingService
{
    public bool TryToAddPlayer(string id, string nickname);
    public bool HasRoomWithName(string roomName);
    public Task RemovePlayer(string id);
    public Task<Room?> CreateTicTacToeRoomAndJoin(string id, string roomName, int dimension, IHubContext<GameHub> hubContext);
    public List<Room> GetRooms();
    public Task<Room?> JoinRoom(string id, int roomId);
    public Task RemovePlayerFromRoom(string id);
    public Task PlayerAction(string contextConnectionId, string type, string data);
}