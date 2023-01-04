﻿using Minesweeper.ConsoleDisplay;
using Minesweeper.Utils;

namespace Minesweeper.UI;

public class Grid : Control, IContainer
{
    public Grid() : base(true)
    {
        Layer = Layer.Background;
        
        Children.ElementChanged += ChildrenOnElementChanged;
        
        Columns.CollectionChanged += ColumnsOnCollectionChanged;
        Rows.CollectionChanged += RowsOnCollectionChanged;
    }
    
    public ObservableList<Control> Children { get; } = new();

    public readonly GridElementCollection<Column> Columns = new();
    public readonly GridElementCollection<Row> Rows = new();

    public ResizeMode ResizeMode { get; set; } = ResizeMode.Both;
    
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Middle;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Middle;
    
    public bool ShowGridLines { get; set; }
    public Color GridLinesColor { get; set; } = Color.White;

    public void SetColumnAndRow(Control control, int column, int row)
    {
        if (column >= Columns.Count || column < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Actual value: {column}");
        }
        
        if (row >= Rows.Count || row < 0)
        {
            throw new InvalidOperationException($"Invalid row index. Actual value: {row}");
        }
        
        if (!Children.Contains(control)) Children.Add(control);

        if (control.AutoResize) control.Resize();

        var pos = new Coord
        {
            X = Columns.GetOffset(column),
            Y = Rows.GetOffset(row)
        };

        control.Position = pos;
    }

    private void ChildrenOnElementChanged(object? sender, CollectionChangedEventArgs<Control> e)
    {
        var element = e.Element;
        
        if (e.ChangeType == ChangeType.Add)
        {
            element.Parent = this;
        }
        else
        {
            e.Element.Parent = null;
        }

        Children.Sort((c1, c2) => c1.Layer.CompareTo(c2.Layer));
    }

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
        var pos = Position + InnerPadding;
        
        for (var i = 0; i < Columns.Count - 1; i++)
        {
            pos.X = Columns.GetOffset(i) + Position.X + Columns[i].Size + Columns.Padding / 2;
            Display.DrawRect(pos, new Coord(1, Height - InnerPadding.Y * 2), GridLinesColor);
        }

        pos = Position + InnerPadding;

        for (var i = 0; i < Rows.Count - 1; i++)
        {
            pos.Y = Rows.GetOffset(i) + Position.Y + Rows[i].Size + Rows.Padding / 2;
            Display.DrawRect(pos, new Coord(Width - InnerPadding.X * 2, 1), GridLinesColor);
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

public sealed class GridElementCollection<T> : ObservableList<T> where T : class, IGridLayoutElement
{
    private int _padding = 1;

    public int Padding
    {
        get => _padding;
        set
        {
            _padding = value;
            OnCollectionChanged();
        }
    }

    public int PaddingCount => Count > 0 ? Count - 1 : 0;
    public int TotalPadding => PaddingCount * Padding;
    public int Size => this.Sum(e => e.Size) + TotalPadding;

    public int GetOffset(int index)
    {
        if (index >= Count || index < 0) throw new IndexOutOfRangeException();

        var offset = 0;
        for (var i = 0; i < index; i++)
        {
            offset += this[i].Size + Padding;
        }

        return offset;
    }
}

public sealed class Column : IGridLayoutElement
{
    public int Size { get; set; }
}

public sealed class Row : IGridLayoutElement
{
    public int Size { get; set; }
}

public interface IGridLayoutElement
{
    public int Size { get; set; }
}
