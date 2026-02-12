using Newtonsoft.Json;

namespace ITask6.Game.TicTacToe;

public class TicTacToeGameBoard
{
    public int[][] Grid { get; private set; }
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
        bool row = true, column = true, diagonal = true, antiDiagonal = true;
        for (int i = 0; i < Dimension; i++)
        {
            if (Grid[_lastMoveRow][i] != _lastMoveValue) row = false;
            if (Grid[i][_lastMoveColumn] != _lastMoveValue) column = false;
            if (Grid[i][i] != _lastMoveValue) diagonal = false;
            if (Grid[i][Dimension - 1 - i] != _lastMoveValue) antiDiagonal = false;
        }
        return row || column || diagonal || antiDiagonal;
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
}