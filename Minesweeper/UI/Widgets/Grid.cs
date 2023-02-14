using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI.Widgets;

public class Grid : Control
{
    public Grid()
    {
        Focusable = false;
        RenderOnItsOwn = true;

        Children.ElementChanged += ChildrenOnElementChanged;
        
        Columns.CollectionChanged += ColumnsOnCollectionChanged;
        Rows.CollectionChanged += RowsOnCollectionChanged;
    }
    
    public ObservableList<Control> Children { get; } = new();

    public readonly GridElementCollection<Column> Columns = new();
    public readonly GridElementCollection<Row> Rows = new();

    public GridResizeDirection GridResizeDirection { get; set; } = GridResizeDirection.Both;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Middle;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Middle;

    public bool ShowGridLines { get; set; }
    public Color GridLinesColor { get; set; } = Color.White;
    public GridLineStyle GridLineStyle { get; set; } = GridLineStyle.Single;

    private readonly List<Entry> _entries = new();

    public void SetColumnAndRow(Control control, int column, int row, bool addChild = true)
    {
        if (column >= Columns.Count || column < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Value: {column}");
        }

        if (row >= Rows.Count || row < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Value: {row}");
        }

        var entry = _entries.FirstOrDefault(e => e.RefTarget == control);

        if (entry == null)
        {
            if (addChild) Children.Add(control);
            
            _entries.Add(new Entry(control, column, row, 1, 1));
        }
        else
        {
            entry.Column = column;
            entry.Row = row;
        }

        if (control is not ContentControl {Content: not null})
        {
            control.Resize();
        }

        AdjustCellSize(control.PaddedSize, column, row);

        var pos = new Vector
        {
            X = Columns.GetOffset(column) + InnerPadding.X,
            Y = Rows.GetOffset(row) + InnerPadding.Y
        };
        
        CalculatePosition(control, pos + control.OuterPadding, column, row);
    }

    public void SetColumnSpanAndRowSpan(Control control, int columnSpan, int rowSpan)
    {
        if (columnSpan > Columns.Count)
        {
            throw new InvalidOperationException($"Invalid column span. Value: {columnSpan}");
        }
        
        if (rowSpan > Rows.Count)
        {
            throw new InvalidOperationException($"Invalid row span. Value: {rowSpan}");
        }

        var entry = _entries.FirstOrDefault(e => e.Reference.Target == control);

        if (entry == null)
        {
            _entries.Add(new Entry(control, 0, 0, columnSpan, rowSpan));
        }
        else
        {
            entry.ColumnSpan = columnSpan;
            entry.RowSpawn = rowSpan;

            if (entry.Column + entry.ColumnSpan > Columns.Count)
            {
                throw new InvalidOperationException($"Invalid column span. Value: {columnSpan}");
            }
            
            if (entry.Row + entry.RowSpawn > Rows.Count)
            {
                throw new InvalidOperationException($"Invalid column span. Value: {columnSpan}");
            }
        }
    }

    private void CalculatePosition(Control control, Vector baseOffset, int column, int row)
    {
        baseOffset.X += HorizontalAlignment switch
        {
            HorizontalAlignment.Middle => Columns[column].Size / 2 - control.PaddedWidth / 2,
            HorizontalAlignment.Right => Columns[column].Size - control.PaddedWidth,
            _ => 0
        };

        baseOffset.Y += VerticalAlignment switch
        {
            VerticalAlignment.Middle => Rows[row].Size / 2 - control.PaddedHeight / 2,
            VerticalAlignment.Bottom => Rows[row].Size - control.PaddedHeight,
            _ => 0
        };
        
        control.Position = baseOffset;
    }

    private void AdjustCellSize(Vector size, int column, int row)
    {
        var shouldMoveContent = false;
        
        if (Columns[column].Size < size.X &&
            GridResizeDirection is GridResizeDirection.Horizontal or GridResizeDirection.Both)
        {
            Columns[column].Size = size.X;
            Width = Columns.Size + InnerPadding.X * 2;

            shouldMoveContent = true;
        }

        if (Rows[row].Size < size.Y &&
            GridResizeDirection is GridResizeDirection.Vertical or GridResizeDirection.Both)
        {
            Rows[row].Size = size.Y;
            Height = Rows.Size + InnerPadding.Y * 2;

            shouldMoveContent = true;
        }

        if (shouldMoveContent) AdjustContentPosition();
    }

    private void AdjustContentPosition()
    {
        foreach (var entry in _entries)
        {
            if (entry.RefTarget is not { } control) continue;

            if (control.ResizeMode == ResizeMode.Expand)
            {
                var expandSize = new Vector(Columns[entry.Column].Size, Rows[entry.Row].Size) - control.OuterPadding * 2;
                
                control.Expand(expandSize);
            }

            var baseOffset = new Vector
            {
                X = Columns.GetOffset(entry.Column),
                Y = Rows.GetOffset(entry.Row)
            };
            
            baseOffset += InnerPadding + control.OuterPadding;
            
            CalculatePosition(control, baseOffset, entry.Column, entry.Row);
        }

        _entries.RemoveAll(e => !e.Reference.IsAlive);
    }

    private void ChildrenOnElementChanged(object? sender, CollectionChangedEventArgs<Control> e)
    {
        if (e.ChangeType == ChangeType.Remove && e.Element.Parent == this)
        {
            e.Element.Parent = null;
        }

        else if (e.ChangeType == ChangeType.Add)
        {
            e.Element.Parent = this;
        }
    }

    private void ColumnsOnCollectionChanged(object? sender, EventArgs e)
    {
        Columns.SetEvenSizes(InnerWidth);
        if (_entries.Count > 0) Resize();
    }

    private void RowsOnCollectionChanged(object? sender, EventArgs e)
    {
        Rows.SetEvenSizes(InnerHeight);
        if (_entries.Count > 0) Resize();
    }

    private void RenderLines()
    {
        var columns = Columns.Count;
        var rows = Rows.Count;
        
        Span<Vector> crosses = stackalloc Vector[(columns - 1) * (rows - 1)];

        var pos  = GlobalPosition + InnerPadding;

        for (var i = 0; i < columns - 1; i++)
        {
            pos.X = Columns.GetOffset(i) + Position.X + Columns[i].Size + 1;

            Display.DrawLine(pos, Vector.Down, InnerHeight, GridLinesColor, CurrentColor,
                GridLines.Symbols[GridLineStyle][GridLineFragment.Vertical]);

            for (var c = i; c < crosses.Length; c += columns - 1)
            {
                crosses[c].X = pos.X;
            }
        }

        pos = GlobalPosition + InnerPadding;

        for (var i = 0; i < rows - 1; i++)
        {
            pos.Y = Rows.GetOffset(i) + Position.Y + Rows[i].Size + 1;
            
            Display.DrawLine(pos, Vector.Right, InnerWidth, GridLinesColor, CurrentColor,
                GridLines.Symbols[GridLineStyle][GridLineFragment.Horizontal]);

            for (var c = 0; c < columns - 1; c++)
            {
                var index = c + i * (columns - 1);

                crosses[index].Y = pos.Y;
            }
        }

        foreach (var crossPosition in crosses)
        {
            Display.Draw(crossPosition.X, crossPosition.Y, GridLines.Symbols[GridLineStyle][GridLineFragment.Cross],
                GridLinesColor, CurrentColor);
        }
    }
    
    public override void Render()
    {
        base.Render();
        
        if (ShowGridLines) RenderLines();
    }

    public override void Remove()
    {
        base.Remove();

        foreach (var child in Children)
        {
            child.Parent = null;
            child.Remove();
        }
        
        Children.Clear();
    }

    public override void Resize()
    {
        Span<int> columnsMinimumWidth = stackalloc int[Columns.Count];
        Span<int> rowsMinimumHeight = stackalloc int[Rows.Count];
        
        foreach (var entry in _entries)
        {
            if (entry.RefTarget is not { } control) continue;

            var minSize = control.ResizeMode == ResizeMode.Expand ? control.MinSize + control.OuterPadding * 2 : control.PaddedSize;

            if (minSize.X > columnsMinimumWidth[entry.Column]) columnsMinimumWidth[entry.Column] = minSize.X;
            if (minSize.Y > rowsMinimumHeight[entry.Row]) rowsMinimumHeight[entry.Row] = minSize.Y;
        }

        for (var i = 0; i < Columns.Count; i++)
        {
            Columns[i].Size = columnsMinimumWidth[i];
        }
        
        for (var i = 0; i < Rows.Count; i++)
        {
            Rows[i].Size = rowsMinimumHeight[i];
        }
        
        Width = Columns.Size + InnerPadding.X * 2;
        Height = Rows.Size + InnerPadding.Y * 2;

        AdjustContentPosition();
    }

    private class Entry
    {
        public Entry(Control control, int column, int row, int columnSpan, int rowSpawn)
        {
            Reference = new WeakReference(control);
            Column = column;
            Row = row;
            ColumnSpan = columnSpan;
            RowSpawn = rowSpawn;
        }

        public WeakReference Reference { get; }
        public Control? RefTarget => Reference.Target as Control;
        
        public int Column { get; set; }
        public int Row { get; set; }

        public int ColumnSpan { get; set; }
        public int RowSpawn { get; set; }
    }
}

public enum HorizontalAlignment
{
    Left,
    Right,
    Middle
}

public enum VerticalAlignment
{
    Top,
    Bottom,
    Middle
}

public enum GridResizeDirection
{
    None,
    Vertical,
    Horizontal,
    Both
}
