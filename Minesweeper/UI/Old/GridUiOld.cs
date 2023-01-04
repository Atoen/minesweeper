namespace Minesweeper.UI.Old;

public sealed class GridUiOld
{
    private readonly Cell[,] _cells;
    public Coord InnerPadding;

    public Coord Pos = Coord.Zero;

    public GridUiOld(int rows, int columns)
    {
        _cells = new Cell[rows, columns];
    }

    public Cell this[int row, int column] => _cells[row, column];

    public Cell this[Coord pos] => _cells[pos.X, pos.Y];

    public void SetCellSize(int row, int column, Coord size, GridAlignment alignment)
    {
        _cells[row, column].ItemSize = size;
        _cells[row, column].Alignment = alignment;
        
        // new size cannot be smaller than the current one
        _cells[row, column].Width = Math.Max(size.X, _cells[row, column].Width);
        _cells[row, column].Height = Math.Max(size.Y, _cells[row, column].Height);

        // Matching the width of the column
        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            _cells[r, column].Width = Math.Max(size.X, _cells[r, column].Width);
        }
        
        // Matching the height of the row
        for (var c = 0; c < _cells.GetLength(1); c++)
        {
            _cells[row, c].Height = Math.Max(size.Y, _cells[row, c].Height);
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var rows = _cells.GetLength(0);
        var columns = _cells.GetLength(1);

        _cells[0, 0].Start = Pos;
        
        // Manually setting first row and first column
        for (var r = 1; r < rows; r++)
        {
            _cells[r, 0].Start.Y = _cells[r - 1, 0].Size.Y + _cells[r - 1, 0].Start.Y + InnerPadding.Y;
            _cells[r, 0].Start.X = Pos.X; // Offset of whole grid
        }
        
        for (var c = 1; c < columns; c++)
        {
            _cells[0, c].Start.X = _cells[0, c - 1].Size.X + _cells[0, c - 1].Start.X + InnerPadding.X;
            _cells[0, c].Start.Y = Pos.Y; // Offset of whole grid
        }

        // Updating rest of the grid
        for (var r = 1; r < rows; r++)
        for (var c = 1; c < columns; c++)
        {
            _cells[r, c].Start = _cells[r - 1, c - 1].Start + _cells[r - 1, c - 1].Size + InnerPadding;
        }
    }

    public struct Cell
    {
        public Coord Start;
        public Coord Size;
        public Coord ItemSize;
        public GridAlignment Alignment;

        public Coord Center => Start + Size / 2;
        public Coord End => Start + Size;
        public int Width
        {
            get => Size.X;
            set => Size.X = value;
        }
        public int Height
        {
            get => Size.Y;
            set => Size.Y = value;
        }
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
