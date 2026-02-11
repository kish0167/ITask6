using System.Security.Cryptography;

namespace ITask6.Game.TicTacToe;

public class TicTacToeRoom() : Room(2)
{
    public TicTacToeGameState State { get; } = TicTacToeGameState.Waiting;

    private Dictionary<TicTacToeGameState, string> _playerSides = new();
    
    protected override void OnGameStarted()
    {
        DecidePlayerSides();
    }

    public override bool CanStartGame()
    {
        return Players.Count == 2;
    }
    private void DecidePlayerSides()
    {
        string[] players = Players.Keys.ToArray();
        if (RandomNumberGenerator.GetInt32(2) == 0)
        {
            _playerSides[TicTacToeGameState.XTurn] = players[0];
            _playerSides[TicTacToeGameState.OTurn] = players[1];
        }
        else
        {
            _playerSides[TicTacToeGameState.XTurn] = players[1];
            _playerSides[TicTacToeGameState.OTurn] = players[0];
        }
    }
}