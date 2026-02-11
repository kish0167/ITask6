namespace ITask6.Game.Services;

public interface IMatchMakingService
{
    public bool TryToAddPlayer(string id, string nickname);
    public void RemovePlayer(string id);
    public Room? CreateRoomAndJoin(string id);
    public List<Room> GetRooms();
    public Room? JoinRoom(string id, int roomId);
    public void LeaveRoom(string id);
}