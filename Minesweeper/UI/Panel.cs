using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI;

public class Panel : Control, IContainer
{
    public ObservableList<Control> Children { get; } = new();
    
}

public class Grid2 : VisualComponent, IContainer
{
    public Grid2() : base(true)
    {
        Layer = Layer.Background;
        
        Children.ElementChanged += OnChildrenElementChanged;
    }

    public ObservableList<Control> Children { get; } = new();

    private readonly RowCollection2 _rows = new();
    private readonly ColumnCollection2 _columns = new();
    private Coord _cellsPadding = new(1, 1);

    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Middle;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Middle;
    
    public bool ShowGridLines { get; set; }

    public Coord CellsPadding
    {
        get => _cellsPadding;
        set
        {
            _cellsPadding = value;
            _columns.Padding = value.X;
            _rows.Padding = value.Y;
        }
    }

    public ResizeMode ResizeMode { get; set; } = ResizeMode.Both;
    
    private void OnChildrenElementChanged(object? sender, CollectionChangedEventArgs<Control> e)
    {
        e.Element.Parent = e.ChangeType == ChangeType.Add ? this : null;
        
        Children.Sort((c1, c2) => c1.Layer.CompareTo(c2.Layer));
    }

    public void SetRowDefinitions(params Row2[] rows)
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

    public void SetColumnDefinitions(params Column2[] columns)
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

    private void ResizeContent()
    {
        Width = _columns.Width;
        Height = _rows.Height;
    }
    
    public void SetRow(VisualComponent component, int rowIndex)
    {
        if (rowIndex > _rows.Count || rowIndex < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Actual value: {rowIndex}");
        }
        
        if (component.AutoResize) component.Resize();

        var selectedRow = _rows[rowIndex];
        if (selectedRow.Height < component.PaddedHeight && ResizeMode is ResizeMode.Both or ResizeMode.Horizontal)
        {
            selectedRow.Height = component.PaddedHeight;
            ResizeContent();
        } 

        var rowPos = 0;
        for (var i = 0; i < rowIndex; i++)
        {
            rowPos += _rows[i].Height;
        }

        rowPos += _rows[rowIndex].Height / 2;

        // component.Position.Y = rowPos + Position.Y - component.Height / 2;
        //
        // component.Position.Y = _rows.PositionOf(rowIndex) + Position.Y;
    }

    public void SetColumn(VisualComponent component, int columnIndex)
    {
        if (columnIndex > _rows.Count || columnIndex < 0)
        {
            throw new InvalidOperationException($"Invalid column index. Actual value: {columnIndex}");
        }
        
        if (component.AutoResize) component.Resize();

        var selectedColumn = _columns[columnIndex];
        if (selectedColumn.Width < component.PaddedWidth && ResizeMode is ResizeMode.Both or ResizeMode.Vertical)
        {
            selectedColumn.Width = component.PaddedWidth;
            
            ResizeContent();
        } 
        
        var columnPos = 0;
        for (var i = 0; i < columnIndex; i++)
        {
            columnPos += _columns[i].Width;
        }

        columnPos += selectedColumn.Width / 2;

        // component.Position.X = columnPos + Position.X - component.Width / 2;
    }

    public void SetRowAndColumn(Control component, int row, int column)
    {
        if (!Children.Contains(component)) Children.Add(component);
        
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

public class RowCollection2 : List<Row2>
{
    public int Padding { get; set; } = 1;
    public int Height => this.Sum(row => row.Height) + PaddingCount * Padding;
    public int PaddingCount => Count > 0 ? Count - 1 : 0;
    
    public int PositionOf(int index)
    {
        if (index >= Count || index < 0) throw new IndexOutOfRangeException();

        var xPos = this.Take(index).Sum(row => row.Height);
        if (index > 0) xPos += (index - 1) * Padding;

        return xPos;
    }

}

public class Row2
{
    public int Height = 0;
}

public class ColumnCollection2 : List<Column2>
{
    public int Padding { get; set; } = 1;
    public int Width => this.Sum(column => column.Width) + PaddingCount * Padding;
    public int PaddingCount => Count > 0 ? Count - 1 : 0;

    public int PositionOf(int index)
    {
        if (index >= Count || index < 0) throw new IndexOutOfRangeException();

        var yPos = this.Take(index).Sum(column => column.Width);
        if (index > 0) yPos += (index - 1) * Padding;

        return yPos;
    }
}


public class Column2
{
    public int Width = 0;
}

public enum GridUnitType
{
    Auto,
    Pixel,
    Weighted
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

public enum ResizeMode
{
    None,
    Vertical,
    Horizontal,
    Both
}