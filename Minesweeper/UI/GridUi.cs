namespace Minesweeper.UI;

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

    public void SetCellSize(int row, int column, Coord size, GridAlignment alignment)
    {
        _cells[row, column].ItemSize = size;
        _cells[row, column].Alignment = alignment;
        
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
        public GridAlignment Alignment;

        public Coord Center => Pos + Size / 2;
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
