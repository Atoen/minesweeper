using System.Text;
using Minesweeper.Display;

namespace Minesweeper.UI;

public class NewFrame
{
    private readonly GridUi _grid;
    private readonly List<(NewWidget, Coord)> _widgets = new();

    public Coord Pos = Coord.Zero;

    public NewFrame(int rows, int columns, int paddingX = 1, int paddingY = 1)
    {
        _grid = new GridUi(rows, columns)
        {
            InsidePaddingX = paddingX,
            InsidePaddingY = paddingY
        };
    }

    public void Grid(NewWidget widget, int row, int column, GridAlignment alignment)
    {
        _widgets.Add((widget, new Coord(row, column)));
        _grid.SetCellSize(row, column, widget.Size);

        // widget.Center = _grid[row, column].Pos;
        AlignToGrid(widget, row, column, alignment);
        
        CheckIfNeedToRedraw();
    }

    public void Place(NewWidget widget, int posX, int posY)
    {
        _widgets.Add((widget, Coord.Zero));

        widget.Center = new Coord(posX, posY);
    }

    private void AlignToGrid(NewWidget widget, int row, int column, GridAlignment alignment)
    {
        var cellPos = _grid[row, column].Pos;
        var cellSize = _grid[row, column].Size;
        
        widget.Anchor = cellPos;

        widget.Center = alignment switch
        {
            GridAlignment.Center => cellPos + cellSize / 2,
            GridAlignment.N =>      cellPos + new Coord(cellSize.X / 2, 0),
            GridAlignment.NE =>     cellPos + new Coord(cellSize.X, (short) 0),
            GridAlignment.NW =>     cellPos,
            GridAlignment.S =>      cellPos + new Coord(cellSize.X / 2, cellSize.Y),
            GridAlignment.SE =>     cellPos + cellSize,
            GridAlignment.SW =>     cellPos + new Coord((short) 0, cellSize.Y),
            GridAlignment.E =>      cellPos + new Coord(cellSize.X, cellSize.Y / 2),
            GridAlignment.W =>      cellPos + new Coord(0, cellSize.Y / 2),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }

    private void CheckIfNeedToRedraw()
    {
        foreach (var (widget, gridPos) in _widgets)
        {
            if (widget.Anchor == _grid[gridPos.X, gridPos.Y].Pos) continue;

            widget.Clear();
            widget.Center = _grid[gridPos.X, gridPos.Y].Pos;
            widget.Render();
        }
    }
}

public sealed class GridUi
{
    private readonly Cell[,] _cells;
    public int InsidePaddingX;
    public int InsidePaddingY;

    public GridUi(int rows, int columns)
    {
        _cells = new Cell[rows, columns];
    }

    public Cell this[int row, int column] => _cells[row, column];

    public void SetCellSize(int row, int column, Coord size)
    {
        _cells[row, column].ItemSize = size;
        
        // new size cannot be smaller than the current one
        _cells[row, column].Size.X = Math.Max(size.X, _cells[row, column].Size.X);
        _cells[row, column].Size.Y = Math.Max(size.Y, _cells[row, column].Size.Y);

        // Matching the width of the column
        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            _cells[r, column].Size.X = Math.Max(size.X, _cells[r, column].Size.X);
        }
        
        // Matching the height of the row
        for (var c = 0; c < _cells.GetLength(1); c++)
        {
            _cells[row, c].Size.Y = Math.Max(size.Y, _cells[row, c].Size.Y);
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var rows = _cells.GetLength(0);
        var columns = _cells.GetLength(1);

        var padding = new Coord(InsidePaddingX, InsidePaddingY);

        _cells[0, 0].Pos = Coord.Zero;
        
        // Manually setting first row and first column
        for (var r = 1; r < rows; r++)
        {
            _cells[r, 0].Pos.Y = (short) (_cells[r - 1, 0].Size.Y + _cells[r - 1, 0].Pos.Y + padding.Y);
        }
        
        for (var c = 1; c < columns; c++)
        {
            _cells[0, c].Pos.X = (short) (_cells[0, c - 1].Size.X + _cells[0, c - 1].Pos.X + padding.X);
        }

        // Updating rest of the grid
        for (var r = 1; r < rows; r++)
        for (var c = 1; c < columns; c++)
        {
            _cells[r, c].Pos = _cells[r - 1, c - 1].Pos + _cells[r - 1, c - 1].Size + padding;
        }
    }

    public struct Cell
    {
        public Coord Pos;
        public Coord Size;
        public Coord ItemSize;
    }
}

public enum GridAlignment
{
    Center,
    N,
    NE,
    NW,
    S,
    SE,
    SW,
    E,
    W,
}