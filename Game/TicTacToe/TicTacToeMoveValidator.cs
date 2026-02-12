namespace ITask6.Game.TicTacToe;

public class TicTacToeMoveValidator
{
    public bool IsValidMove(TicTacToeGameBoard board, string playerId, string action, 
        TicTacToeStateManager stateManager, TicTacToePlayerManager playerManager, out int row, out int col)
    {
        if (!TryParseMove(board, action, out row, out col)) return false;
        if (!board.IsCellEmpty(row, col)) return false;
        if (!playerManager.IsCurrentPlayerTurn(playerId, stateManager.CurrentStage)) return false;
        
        return true;
    }
    
    private bool TryParseMove(TicTacToeGameBoard board, string action, out int row, out int col)
    {
        row = col = -1;
        if (!int.TryParse(action, out int move)) return false;
        if (move < 0 || move >= board.Dimension * board.Dimension) return false;
        row = move / board.Dimension;
        col = move % board.Dimension;
        return true;
    }
}