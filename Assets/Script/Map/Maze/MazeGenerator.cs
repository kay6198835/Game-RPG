using System;
using System.Collections.Generic;
[System.Serializable]
public class MazeGenerator
{
    private int _rows = 10;
    private int _columns = 10;
    private Cell[] Gird;
    private Stack<Cell> stack = new Stack<Cell>();
    private readonly Random _random = new Random();
    public Cell Start { get; private set; }
    public Cell End { get; private set; }
    public void Generator(int rows, int cols)
    {
        _rows = rows;
        _columns = cols;
        Gird = new Cell[rows * cols];
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
    private Cell GetValue(int row, int col)
    {
        return Gird[row * this._columns + col];
    }
    private void SetValue(int row, int col, Cell cell)
    {
        Gird[row * this._columns + col] = cell;
    }
    private Cell GetRandomCell()
    {
        int row = _random.Next(0, _rows);
        int col = _random.Next(0, _columns);
        return grid[row, col];
    }
    private Cell[] Generate()
    {
        Start = GetRandomCell();
        Start.Visited = true;
        stack.Push(Start);

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
            End = next;
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
            current.Doors[GameConstants.Direction.Name.TOP] = STATUS_DOOR.ENEBLE;
            next.Doors[GameConstants.Direction.Name.BOTTOM] = STATUS_DOOR.BE_OPEN;
        }
        else if (rowDiff == 1)
        {
            current.Doors[GameConstants.Direction.Name.BOTTOM] = STATUS_DOOR.ENEBLE;
            next.Doors[GameConstants.Direction.Name.TOP] = STATUS_DOOR.BE_OPEN;
        }
        else if (colDiff == -1)
        {
            current.Doors[GameConstants.Direction.Name.LEFT] = STATUS_DOOR.ENEBLE;
            next.Doors[GameConstants.Direction.Name.RIGHT] = STATUS_DOOR.BE_OPEN;
        }
        else if (colDiff == 1)
        {
            current.Doors[GameConstants.Direction.Name.RIGHT] = STATUS_DOOR.ENEBLE;
            next.Doors[GameConstants.Direction.Name.LEFT] = STATUS_DOOR.BE_OPEN;
        }
    }
}

