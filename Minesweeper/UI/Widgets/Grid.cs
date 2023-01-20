using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI.Widgets;

public class Grid : Control
{
    public Grid()
    {
        IsFocusable = false;
        
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
    
    private record ControlEntry(Control Control, Coord GridPos, Coord GridSpan);
    private readonly List<ControlEntry> _entries = new();

    public void SetColumnAndRow(Control control, int column, int row)
    {
        if (column >= Columns.Count || column < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Value: {column}");
        }
        
        if (row >= Rows.Count || row < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Value: {row}");
        }

        var entry = new ControlEntry(control, new Coord(column, row), new Coord(1, 1));

        if (!Children.Contains(control))
        {
            Children.Add(control);
            
            _entries.Add(entry);
        }
        else
        {
            var index = _entries.FindIndex(e => e.Control == control);

            _entries[index] = entry;
        }

        if (control is not ContentControl contentControl || contentControl.Content == null)
        {
            control.Resize();
        }
        
        AdjustCellSize(control.PaddedSize, column, row);
        
        var pos = new Coord
        {
            X = Columns.GetOffset(column) + InnerPadding.X,
            Y = Rows.GetOffset(row) + InnerPadding.Y
        };
        
        CalculatePosition(entry, pos + control.OuterPadding);
    }

    private void CalculatePosition(ControlEntry entry, Coord baseOffset)
    {
        baseOffset.X += HorizontalAlignment switch
        {
            HorizontalAlignment.Middle => Columns[entry.GridPos.X].Size / 2 - entry.Control.Width / 2,
            HorizontalAlignment.Right => Columns[entry.GridPos.X].Size - entry.Control.Width,
            _ => 0
        };

        baseOffset.Y += VerticalAlignment switch
        {
            VerticalAlignment.Middle => Rows[entry.GridPos.Y].Size / 2 - entry.Control.Height / 2,
            VerticalAlignment.Bottom => Rows[entry.GridPos.Y].Size - entry.Control.Height,
            _ => 0
        };
        
        entry.Control.Position = baseOffset;
    }

    private void AdjustCellSize(Coord size, int column, int row)
    {
        if (Columns[column].Size < size.X &&
            GridResizeDirection is GridResizeDirection.Horizontal or GridResizeDirection.Both)
        {
            Columns[column].Size = size.X;
            Width = Columns.Size + InnerPadding.X * 2;
        }

        if (Rows[row].Size >= size.Y ||
            GridResizeDirection is not (GridResizeDirection.Vertical or GridResizeDirection.Both)) return;
        
        Rows[row].Size = size.Y;
        Height = Rows.Size + InnerPadding.Y * 2;
    }

    private void ChildrenOnElementChanged(object? sender, CollectionChangedEventArgs<Control> e) => 
        e.Element.Parent = e.ChangeType == ChangeType.Add ? this : null;

    private void ColumnsOnCollectionChanged(object? sender, EventArgs e) => 
        SetEvenSizes(Columns, Width - InnerPadding.X * 2);

    private void RowsOnCollectionChanged(object? sender, EventArgs e) =>
        SetEvenSizes(Rows, Height - InnerPadding.Y * 2);

    private static void SetEvenSizes<T>(GridElementCollection<T> collection, int totalSize)
        where T : class, IGridLayoutElement
    {
        if (totalSize == 0) return;

        var elementsCount = collection.Count;
        
        var sizeToDivide = totalSize - collection.TotalPadding;
        var size = sizeToDivide / elementsCount;
        var remainder = sizeToDivide % elementsCount;

        for (var i = 0; i < elementsCount; i++)
        {
            var elementSize = size;
            if (remainder > 0)
            {
                elementSize++;
                remainder--;
            }

            collection[i].Size = elementSize;
        }
    }

    private void RenderLines()
    {
        Span<Coord> crosses = stackalloc Coord[(Columns.Count - 1) * (Rows.Count - 1)];

        var pos  = GlobalPosition + InnerPadding;

        for (var i = 0; i < Columns.Count - 1; i++)
        {
            pos.X = Columns.GetOffset(i) + Position.X + Columns[i].Size + 1;

            Display.DrawLine(pos, Coord.Down, Height - InnerPadding.Y * 2, GridLinesColor, CurrentColor,
                GridLines.Symbols[GridLineStyle][GridLineFragment.Vertical]);

            for (var c = i; c < crosses.Length; c += Columns.Count - 1)
            {
                crosses[c].X = pos.X;
            }
        }

        pos = GlobalPosition + InnerPadding;

        for (var i = 0; i < Rows.Count - 1; i++)
        {
            pos.Y = Rows.GetOffset(i) + Position.Y + Rows[i].Size + 1;
            
            Display.DrawLine(pos, Coord.Right, Width - InnerPadding.X * 2, GridLinesColor, CurrentColor,
                GridLines.Symbols[GridLineStyle][GridLineFragment.Horizontal]);

            for (var c = 0; c < Columns.Count - 1; c++)
            {
                var index = c + i * (Columns.Count - 1);

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

        foreach (var child in Children)
        {
            child.Render();
        }
    }

    public override void Remove()
    {
        base.Remove();

        foreach (var child in Children)
        {
            child.Remove();
        }
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
