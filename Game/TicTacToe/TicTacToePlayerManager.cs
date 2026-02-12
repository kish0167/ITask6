using System.Security.Cryptography;

namespace ITask6.Game.TicTacToe;

public class TicTacToePlayerManager
{
    private readonly Dictionary<TicTacToeGameState, string> _playerSides = new();
    
    public void AssignSides(string player1, string player2)
    {
        bool xGoesFirst = RandomNumberGenerator.GetInt32(2) == 0;
        
        _playerSides[TicTacToeGameState.XTurn] = xGoesFirst ? player1 : player2;
        _playerSides[TicTacToeGameState.OTurn] = xGoesFirst ? player2 : player1;
    }
    
    public string GetPlayerId(TicTacToeGameState state) => _playerSides[state];

    public bool IsCurrentPlayerTurn(string playerId, TicTacToeGameState currentState)
    {
        return _playerSides.TryGetValue(currentState, out var id) && id == playerId;
    }

    public string? GetPlayerSide(string id)
    {
        if (_playerSides.TryGetValue(TicTacToeGameState.XTurn, out var xId) && xId == id)
            return "x";
        if (_playerSides.TryGetValue(TicTacToeGameState.OTurn, out var oId) && oId == id)
            return "o";
        return null;
    }


    public string GetWinnerId(TicTacToeGameState lastTurnState)
    {
        return _playerSides[lastTurnState];
    }
}