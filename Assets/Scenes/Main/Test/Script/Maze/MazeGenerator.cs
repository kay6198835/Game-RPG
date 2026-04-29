using System;
using System.Collections.Generic;
[System.Serializable]
public class MazeGenerator
{
    private int _rows = 10;
    private int _columns = 10;
    public Cell [] Gird;
    private Stack<Cell> stack = new Stack<Cell>();
    private readonly Random _random = new Random();

    public void Generator(int rows, int cols)
    {
        _rows = rows;
        _columns = cols;
        Gird = new Cell[rows*cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Cell cell = new Cell(r, c);
                SetValue(r, c, cell);
            }
        }
        Generate();
    }

    public Cell GetValue (int row, int col) {
        return Gird[row * this._columns+ col];    
    }

    public void SetValue (int row, int col,Cell cell)
    {
        Gird[row * this._columns + col] = cell;
    }

    private Cell[] Generate()
    {
        var start = GetValue(0,0);
        start.Visited = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var next = neighbors[_random.Next(neighbors.Count)];
            RemoveWall(current, next);
            next.Visited = true;
            stack.Push(next);
        }

        return this.Gird;
    }

    private List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        var result = new List<Cell>();
        int r = cell.Row;
        int c = cell.Column;

        if (r > 0 && !GetValue(r - 1, c).Visited) result.Add(GetValue(r - 1, c));
        if (c < _columns - 1 && !GetValue(r, c + 1).Visited) result.Add(GetValue(r, c + 1));
        if (r < _rows - 1 && !GetValue(r + 1, c).Visited) result.Add(GetValue(r + 1, c));
        if (c > 0 && !GetValue(r, c - 1).Visited) result.Add(GetValue(r, c - 1));

        return result;
    }
    private void RemoveWall(Cell current, Cell next)
    {
        int rowDiff = next.Row - current.Row;
        int colDiff = next.Column - current.Column;

        if (rowDiff == -1)
        {
            current.Top = (int)STATUS_DOOR.OPEN;
            next.Bottom = (int)STATUS_DOOR.BE_OPEN;
        }
        else if (rowDiff == 1)
        {
            current.Bottom = (int)STATUS_DOOR.OPEN;
            next.Top = (int)STATUS_DOOR.BE_OPEN;
        }
        else if (colDiff == -1)
        {
            current.Left = (int)STATUS_DOOR.OPEN;
            next.Right = (int)STATUS_DOOR.BE_OPEN;
        }
        else if (colDiff == 1)
        {
            current.Right = (int)STATUS_DOOR.OPEN;
            next.Left = (int)STATUS_DOOR.BE_OPEN;
        }
    }
}

