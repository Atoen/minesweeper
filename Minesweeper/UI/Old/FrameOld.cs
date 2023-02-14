using Minesweeper.UI.Widgets;

namespace Minesweeper.UI.Old;

public class FrameOld
{
    private record WidgetEntry(Widget Widget, Vector GridPos, Vector GridSpan);
    
    private readonly GridUiOld _grid;
    private readonly List<WidgetEntry> _widgets = new();
    private readonly Vector _padding;
    private Vector _pos = Vector.Zero;

    public Vector Pos
    {
        get => _pos;
        set
        {
            _pos = value;
            _grid.Pos = value;
        }
    }

    public FrameOld(int rows, int columns, int paddingX = 1, int paddingY = 1)
    {
        _padding.X = paddingX;
        _padding.Y = paddingY;
        
        _grid = new GridUiOld(rows, columns)
        {
            InnerPadding = _padding
        };
    }
    
    public void Grid(Widget widget, int row, int column, int rowSpan, int columnSpan, GridAlignment alignment)
    {
        _widgets.Add(new WidgetEntry(widget, (row, column), (rowSpan, columnSpan)));

        var multiCell = rowSpan != 1 || columnSpan != 1;

        if (widget.GridResizeDirection != GridResizeDirection.None && !multiCell)
        {
            FillWidget(widget, _grid[row, column].Size);
        }
        
        if (multiCell)
        {
            SetMultipleCellSize((row, column), rowSpan, columnSpan, widget.PaddedSize, alignment);
            AlignToGridMultiCell(widget, row, column, rowSpan, columnSpan);
        }
        else
        {
            _grid.SetCellSize(row, column, widget.PaddedSize, alignment);
            AlignToGrid(widget, row, column, alignment);
        }
        
        CheckIfNeedToRedraw();
    }

    private void SetMultipleCellSize(Vector firstCellPos, int rowSpan, int columnSpan, Vector widgetSize, GridAlignment alignment)
    {
        var paddings = new Vector((columnSpan - 1) * _padding.X, (rowSpan - 1) * _padding.Y);

        var lastCellPos = firstCellPos + (rowSpan - 1, columnSpan - 1);
        var currentSizeAvailable = _grid[lastCellPos].End - _grid[firstCellPos].Start;

        // Setting cells horizontally
        if (currentSizeAvailable.X < widgetSize.X)
        {
            var sizeToDivide = widgetSize.X - paddings.X;
            
            var cellWidth = sizeToDivide / columnSpan;
            var remainder = sizeToDivide % columnSpan;

            for (var i = 0; i < columnSpan; i++)
            {
                var size = cellWidth + remainder;
                if (remainder > 0) remainder--;
                
                _grid.SetCellSize(firstCellPos.X, firstCellPos.Y + i, Vector.Right * size, alignment);
            }
        }

        // Setting cells vertically
        if (currentSizeAvailable.Y < widgetSize.Y)
        {
            var sizeToDivide = widgetSize.Y - paddings.Y;
            
            var cellWidth = sizeToDivide / rowSpan;
            var remainder = sizeToDivide % rowSpan;

            for (var i = 0; i < rowSpan; i++)
            {
                var size = cellWidth + remainder;
                if (remainder > 0) remainder--;
                
                _grid.SetCellSize(firstCellPos.X + i, firstCellPos.Y, Vector.Down * size, alignment);
            }
        }
    }
    
    public void Place(Widget widget, int posX, int posY)
    {
        _widgets.Add(new WidgetEntry(widget, Vector.Zero, Vector.Zero));
        
        // widget.Anchor = new Coord(posX, posY);
        // widget.Offset = Coord.Zero;
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

        if (entry is null) return;
        
        widget.Remove();
        _widgets.Remove(entry);
    }

    private void FillWidget(Widget widget, Vector cellSize)
    {
        // switch (widget.GridResizeMode)
        // {
        //     case ResizeMode.Vertical:
        //         widget.Height = Math.Max(widget.Height, cellSize.Y);
        //         break;
        //     
        //     case ResizeMode.Horizontal:
        //         widget.Width = Math.Max(widget.Width, cellSize.X);
        //         break;
        //     
        //     case ResizeMode.Both:
        //         widget.Size = widget.Size.ExpandTo(cellSize);
        //         break;
        //     
        //     case ResizeMode.None:
        //         break;
        //     
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(widget), widget.GridResizeMode, null);
        // }
    }

    private void CheckIfNeedToRedraw()
    {
        foreach (var (widget, gridPos, gridSpan) in _widgets)
        {
            // Single cell widgets
            if (gridSpan == (1, 1))
            {
                // if (widget.Anchor == _grid[gridPos].Center && widget.PaddedSize == _grid[gridPos].Size) continue;

                widget.Clear();
                
                FillWidget(widget, _grid[gridPos].Size - widget.OuterPadding * 2);
                
                AlignToGrid(widget, gridPos.X, gridPos.Y, _grid[gridPos].Alignment);
                
                continue;
            }

            // multi cell widgets
            var startPos = _grid[gridPos].Start;
            
            var endCell = _grid[gridPos + gridSpan - (1, 1)];
            var endPos = endCell.Start + endCell.Size;

            var anchor = (startPos + endPos) / 2;
            var size = endPos - startPos;

            // if (widget.Anchor == anchor && widget.Size == size) continue;
            //
            // widget.Clear();
            //
            // widget.Size = size;
            // widget.Anchor = anchor;
            // widget.Offset = -widget.Size / 2;
        }
    }

    private void AlignToGrid(Widget widget, int row, int column, GridAlignment alignment)
    {
        var cell = _grid[row, column];
        var cellSize = cell.Size;
        var widgetSize = widget.Size;
        
        // widget.Anchor = cell.Center;
        //
        // widget.Offset = alignment switch
        // {
        //     GridAlignment.Center => -widgetSize / 2,
        //     GridAlignment.N => Coord.Up * (cellSize.Y / 2) + Coord.Left * (widgetSize.X / 2),
        //     GridAlignment.NE => Coord.Up * (cellSize.Y / 2) + Coord.Right * (cellSize.X / 2 - widgetSize.X),
        //     GridAlignment.NW => -cellSize / 2,
        //     GridAlignment.S => Coord.Left * (widgetSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
        //     GridAlignment.SE => Coord.Down * (cellSize.Y / 2 - widgetSize.Y) + Coord.Right * (cellSize.X / 2 - widgetSize.X),
        //     GridAlignment.SW => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
        //     GridAlignment.E => Coord.Right * (cellSize.X / 2 - widgetSize.X) + Coord.Up * (widgetSize.Y / 2),
        //     GridAlignment.W => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y / 2),
        //     _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        // };
    }

    private void AlignToGridMultiCell(Widget widget, int row, int column, int rowSpan, int columnSpan)
    {
        var startPos = _grid[row, column].Start;

        var endCell = _grid[row + rowSpan - 1, column + columnSpan - 1];
        var endPos = endCell.Start + endCell.Size;
        //
        // widget.Size = endPos - startPos;
        // widget.Anchor = (startPos + endPos) / 2;
        // widget.Offset = -widget.Size / 2;
    } 
}
