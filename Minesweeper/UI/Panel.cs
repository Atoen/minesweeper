using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI;

public class Panel : Control, IContainer
{
    public ObservableList<VisualComponent> Children { get; } = new();

    public Panel(IContainer? parent = null) : base(parent)
    {
        
    }
}

public class Grid : VisualComponent, IContainer
{
    public Grid() : base(true)
    {
        Layer = Layer.Background;
        
        Children.Changed += OnChildrenChanged;
    }

    private void OnChildrenChanged(object? sender, CollectionChangedEventArgs<VisualComponent> e)
    {
        e.Element.Parent = e.ChangeType == ChangeType.Add ? this : null;
        
        Children.Sort((c1, c2) => c1.Layer.CompareTo(c2.Layer));
    }

    public ObservableList<VisualComponent> Children { get; } = new();

    private readonly List<Row> _rows = new();
    private readonly List<Column> _columns = new();

    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Middle;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Middle;
    
    public bool ShowGridLines { get; set; }
    public Coord CellsPadding = new(1, 1);

    public void SetRowDefinitions(params Row[] rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        
        SetRowSizes();
    }

    private void SetRowSizes()
    {
        if (Height == 0) return;

        var rows = _rows.Count;
        var padding = (rows - 1) * CellsPadding.Y;
        
        var sizeToDivide = Height - padding;

        var rowHeight = sizeToDivide / rows;
        var remainder = sizeToDivide % rows;

        for (var i = 0; i < rows; i++)
        {
            var height = rowHeight + remainder;
            if (remainder > 0) remainder--;

            _rows[i].Height = height;
        }
    }

    public void SetColumnDefinitions(params Column[] columns)
    {
        _columns.Clear();
        _columns.AddRange(columns);
        
        SetColumnSizes();
    }

    private void SetColumnSizes()
    {
        if (Width == 0) return;

        var columns = _columns.Count;
        var padding = (columns - 1) * CellsPadding.X;
        
        var sizeToDivide = Width - padding;

        var columWidth = sizeToDivide / columns;
        var remainder = sizeToDivide % columns;

        for (var i = 0; i < columns; i++)
        {
            var width = columWidth + remainder;
            if (remainder > 0) remainder--;

            _columns[i].Width = width;
        }
    }
    
    public void SetRow(VisualComponent component, int row)
    {
        if (row > _rows.Count || row < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Actual value: {row}");
        }
        
        if (component.AutoResize) component.Resize();

        var rowPos = 0;
        for (var i = 0; i < row; i++)
        {
            rowPos += _rows[i].Height;
        }

        rowPos += _rows[row].Height / 2;

        component.Position.Y = rowPos + Position.Y - component.Height / 2;
    }

    public void SetColumn(VisualComponent component, int column)
    {
        if (column > _rows.Count || column < 0)
        {
            throw new InvalidOperationException($"Invalid column index. Actual value: {column}");
        }
        
        if (component.AutoResize) component.Resize();
        
        var columnPos = 0;
        for (var i = 0; i < column; i++)
        {
            columnPos += _columns[i].Width;
        }

        columnPos += _columns[column].Width / 2;

        component.Position.X = columnPos + Position.X - component.Width / 2;
    }

    public void SetRowAndColumn(VisualComponent component, int row, int column)
    {
        SetRow(component, row);
        SetColumn(component, column);
    }

    public void SetRowSpan(VisualComponent component, int rowSpan)
    {
        
    }

    public void SetColumnSpan(VisualComponent component, int columnSpan)
    {
        
    }

    public override void Render()
    {
        base.Render();

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

public class Row
{
    public int Height = 0;
}

public class Column
{
    public int Width = 0;
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

public enum FillMode
{
    None,
    Vertical,
    Horizontal,
    Both
}