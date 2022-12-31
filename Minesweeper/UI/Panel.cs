using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Panel : Control, IContainer
{
    public List<VisualElement> Children { get; } = new();

    public Panel(IContainer? parent = null) : base(parent)
    {
        
    }
}

public class Grid : VisualElement, IContainer
{
    public Grid() : base(true)
    {
        Layer = Layer.Background;
    }
    
    public List<VisualElement> Children { get; } = new();

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
    }

    public void SetRow(VisualElement element, int row)
    {
        if (row > _rows.Count || row < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Actual value: {row}");
        }
        
        if (element.AutoResize) element.Resize();

        element.Position.Y = _rows[row].Height + Position.Y;
    }

    public void SetColumn(VisualElement element, int column)
    {
        if (column > _rows.Count || column < 0)
        {
            throw new InvalidOperationException($"Invalid column index. Actual value: {column}");
        }
        
        if (element.AutoResize) element.Resize();

        element.Position.X = _columns[column].Width + Position.X;
    }

    public void SetRowAndColumn(VisualElement element, int row, int column)
    {
        SetRow(element, row);
        SetColumn(element, column);
    }

    public void SetRowSpan(VisualElement element, int rowSpan)
    {
        
    }

    public void SetColumnSpan(VisualElement element, int columnSpan)
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