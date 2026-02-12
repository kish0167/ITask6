using System.Security.Cryptography;

namespace ITask6.Game.TicTacToe;

public class TicTacToePlayerManager
{
    private readonly Dictionary<TicTacToeGameStage, string> _playerSides = new();
    
    public void AssignSides(string player1, string player2)
    {
        bool xGoesFirst = RandomNumberGenerator.GetInt32(2) == 0;
        
        _playerSides[TicTacToeGameStage.XTurn] = xGoesFirst ? player1 : player2;
        _playerSides[TicTacToeGameStage.OTurn] = xGoesFirst ? player2 : player1;
    }

    public string GetPlayerId(TicTacToeGameStage stage)
    {
        return _playerSides[stage];
    } 

    public bool IsCurrentPlayerTurn(string playerId, TicTacToeGameStage currentStage)
    {
        return _playerSides.TryGetValue(currentStage, out var id) && id == playerId;
    }

    public string? GetPlayerSide(string id)
    {
        if (_playerSides.TryGetValue(TicTacToeGameStage.XTurn, out var xId) && xId == id)
            return "x";
        if (_playerSides.TryGetValue(TicTacToeGameStage.OTurn, out var oId) && oId == id)
            return "o";
        return null;
    }
}