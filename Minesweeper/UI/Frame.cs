using System.Text;
using Minesweeper.Display;

namespace Minesweeper.UI;

public class Frame
{
    private readonly GridUi _grid;
    private readonly List<WidgetEntry> _widgets = new();
    private readonly Coord _padding;
    private Coord _pos = Coord.Zero;

    public Coord Pos
    {
        get => _pos;
        set
        {
            _pos = value;
            _grid.Pos = value;
        }
    }

    public Frame(int rows, int columns, int paddingX = 1, int paddingY = 1)
    {
        _padding.X = paddingX;
        _padding.Y = paddingY;
        
        _grid = new GridUi(rows, columns)
        {
            InsidePaddingX = paddingX,
            InsidePaddingY = paddingY,
        };
    }
    
    public void Grid(Widget widget, int row, int column, int rowSpan, int columnSpan, GridAlignment alignment)
    {
        _widgets.Add(new WidgetEntry(widget, (row, column), (rowSpan, columnSpan)));

        var multiCell = rowSpan != 1 || columnSpan != 1;

        if (widget.Fill != FillMode.None && !multiCell)
        {
            FillWidget(widget, _grid[row, column].Size);
        }
        else if (!multiCell)
        {
            var newCellSize = widget.Size + widget.OuterPadding * 2;
            _grid.SetCellSize(row, column, newCellSize, alignment);  
        }
        
        if (multiCell)
        {
            AlignToGridMultiCell(widget, row, column, rowSpan, columnSpan);
        }
        else
        {
            AlignToGrid(widget, row, column, alignment);
        }
        
        CheckIfNeedToRedraw();
    }

    public void Place(Widget widget, int posX, int posY)
    {
        _widgets.Add(new WidgetEntry(widget, Coord.Zero, Coord.Zero));


        widget.Anchor = new Coord(posX, posY);
        widget.Offset = Coord.Zero;
    }

    public void Clear()
    {
        foreach (var entry in _widgets)
        {
            entry.Widget.Remove();
        }
        
        _widgets.Clear();
    }

    public void Remove(Widget widget)
    {
        var entry = _widgets.FirstOrDefault(e => e.Widget == widget);
        widget.Remove();

        if (entry is not null) _widgets.Remove(entry);
    }

    private void FillWidget(Widget widget, Coord cellSize)
    {
        switch (widget.Fill)
        {
            case FillMode.Vertical:
                widget.Size.Y = cellSize.Y;
                break;
            
            case FillMode.Horizontal:
                widget.Size.X = cellSize.X;
                break;
            
            case FillMode.Both:
                widget.Size = cellSize;
                break;

            case FillMode.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(widget), widget.Fill, null);
        } 
    }
    
    private void CheckIfNeedToRedraw()
    {
        // foreach (var (widget, gridPos) in _widgets)
        // {
        //     if (widget.Anchor == _grid[gridPos.X, gridPos.Y].Center) continue;
        //
        //     widget.Clear();
        //     
        //     AlignToGrid(widget, gridPos.X, gridPos.Y, _grid[gridPos.X, gridPos.Y].Alignment);
        // }

        foreach (var (widget, pos, gridSpan) in _widgets)
        {
            if (gridSpan == (1, 1))
            {
                // Single cell widgets
                if (widget.Anchor == _grid[pos].Center) continue;
                
                widget.Clear();
                
                AlignToGrid(widget, pos.X, pos.Y, _grid[pos].Alignment);
                
                continue;
            }

            // multi cell widgets
            var startPos = _grid[pos].Pos;
            
            var endCell = _grid[pos + gridSpan - (1, 1)];
            var endPos = endCell.Pos + endCell.Size;

            var anchor = (startPos + endPos) / 2;
            var size = endPos - startPos;
            
            if (widget.Anchor == anchor && widget.Size == size) continue;
            
            widget.Clear();
            
            widget.Size = size;
            widget.Anchor = anchor;
            widget.Offset = -widget.Size / 2;
        }
    }

    private void AlignToGrid(Widget widget, int row, int column, GridAlignment alignment)
    {
        var cell = _grid[row, column];
        var cellSize = cell.Size;
        var widgetSize = widget.Size;
        
        widget.Anchor = cell.Center;

        widget.Offset = alignment switch
        {
            GridAlignment.Center => -widgetSize / 2,
            GridAlignment.N => Coord.Up * (cellSize.Y / 2) + Coord.Left * (widgetSize.X / 2),
            GridAlignment.NE => Coord.Up * (cellSize.Y / 2) + Coord.Right * (cellSize.X / 2 - widgetSize.X),
            GridAlignment.NW => -cellSize / 2,
            GridAlignment.S => Coord.Left * (widgetSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
            GridAlignment.SE => Coord.Down * (cellSize.Y / 2 - widgetSize.Y) + Coord.Right * (cellSize.X / 2 - widgetSize.X),
            GridAlignment.SW => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
            GridAlignment.E => Coord.Right * (cellSize.X / 2 - widgetSize.X) + Coord.Up * (widgetSize.Y / 2),
            GridAlignment.W => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y / 2),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }

    private void AlignToGridMultiCell(Widget widget, int row, int column, int rowSpan, int columnSpan)
    {
        var startPos = _grid[row, column].Pos;

        var endCell = _grid[row + rowSpan - 1, column + columnSpan - 1];
        var endPos = endCell.Pos + endCell.Size;

        widget.Size = endPos - startPos;
        widget.Anchor = (startPos + endPos) / 2;
        widget.Offset = -widget.Size / 2;
    }

    private record WidgetEntry(Widget Widget, Coord GridPos, Coord GridSpan);
}
