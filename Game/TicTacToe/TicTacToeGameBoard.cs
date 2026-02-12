using Newtonsoft.Json;

namespace ITask6.Game.TicTacToe;

public class TicTacToeGameBoard
{
    public int[][] Grid { get; }
    public int Dimension { get; }

    private int _lastMoveRow;
    private int _lastMoveColumn;
    private int _lastMoveValue;
    
    private const int MinDimension = 3;
    private const int MaxDimension = 10;

    public TicTacToeGameBoard(int dimension)
    {
        Dimension = Math.Clamp(dimension, MinDimension, MaxDimension);
        Grid = new int[Dimension][];
        Reset();
    }

    public void PlaceMove(int row, int column, int value)
    {
        Grid[row][column] = value;
        _lastMoveRow = row;
        _lastMoveColumn = column;
        _lastMoveValue = value;
    }
    
    public bool WinCondition()
    {
        if (Dimension > 4) return BigBoardWinCondition();
        bool[] flags = [true, true, true, true];
        for (int i = 0; i < Dimension; i++)
        {
            if (Grid[_lastMoveRow][i] != _lastMoveValue) flags[0] = false;
            if (Grid[i][_lastMoveColumn] != _lastMoveValue) flags[1] = false;
            if (Grid[i][i] != _lastMoveValue) flags[2] = false;
            if (Grid[i][Dimension - 1 - i] != _lastMoveValue) flags[3] = false;
        }
        return flags[0] || flags[1] || flags[2] || flags[3];
    }

    private bool BigBoardWinCondition()
    {
        int[] flags = [0,0,0,0];
        for (int i = 0; i < Dimension; i++)
        {
            Increment(flags);
            if (Grid[_lastMoveRow][i] != _lastMoveValue) flags[0] = 0;
            if (Grid[i][_lastMoveColumn] != _lastMoveValue) flags[1] = 0;
            CheckDiagonalsOnBigBoard(i, flags);
            if (AnyAbove3(flags)) return true;
        }
        return false;
    }

    private void CheckDiagonalsOnBigBoard(int i, int[] flags)
    {
        int rd = Math.Max(0, _lastMoveRow - _lastMoveColumn) + i;
        int cd = Math.Max(0, _lastMoveColumn - _lastMoveRow) + i;
        int rad = Math.Min(_lastMoveRow + _lastMoveColumn, Dimension - 1) - i;
        int cad = Math.Max(_lastMoveRow + _lastMoveColumn - Dimension, 0) + i;
        if (rd >= Dimension || cd >= Dimension || Grid[rd][cd] != _lastMoveValue) flags[2] = 0;
        if (rad < 0 || cad >= Dimension || Grid[rad][cad] != _lastMoveValue) flags[3] = 0;
    }

    public void Reset()
    {
        for (int i = 0; i < Dimension; i++)
        {
            Grid[i] = new int[Dimension];
        }
    }
    
    public bool IsCellEmpty(int row, int col) => Grid[row][col] == 0;

    public bool IsFull()
    {
        for (int i = 0; i < Dimension; i++)
        {
            for (int j = 0; j < Dimension; j++)
            {
                if (Grid[i][j] == 0) return false;
            }
        }
        return true;
    }
    
    private void Increment(int[] flags)
    {
        for (int i = 0; i < flags.Length; i++)
        {
            flags[i]++;
        }
    }
    private bool AnyAbove3(int[] flags)
    {
        for (int i = 0; i < flags.Length; i++)
        {
            if (flags[i] > 3)
            {
                return true;
            }
        }
        return false;
    }
}