using System.Text;
using Minesweeper.Display;

namespace Minesweeper.UI;

public class Frame
{
    private readonly GridUi _grid;
    private readonly List<(Widget, Coord)> _widgets = new();

    private readonly Coord _padding;
    
    public Coord Pos = Coord.Zero;

    public Frame(int rows, int columns, int paddingX = 1, int paddingY = 1)
    {
        _padding.X = paddingX;
        _padding.Y = paddingY;
        
        _grid = new GridUi(rows, columns)
        {
            InsidePaddingX = paddingX,
            InsidePaddingY = paddingY
        };
    }

    public void Grid(Widget widget, int row, int column, GridAlignment alignment)
    {
        _widgets.Add((widget, new Coord(row, column)));
        _grid.SetCellSize(row, column, widget.Size, alignment);
        
        AlignToGrid(widget, row, column, alignment);
        
        CheckIfNeedToRedraw();
    }

    public void Place(Widget widget, int posX, int posY)
    {
        _widgets.Add((widget, Coord.Zero));

        widget.Anchor = new Coord(posX, posY);
        widget.Offset = Coord.Zero;
    }

    public void Clear()
    {
        foreach (var (widget, _) in _widgets)
        {
            widget.Remove();
        }
        
        _widgets.Clear();
    }

    public void Remove(Widget widget)
    {
        var tuple = _widgets.FirstOrDefault(t => t.Item1 == widget);
        widget.Remove();
        _widgets.Remove(tuple);
    }
    
    private void CheckIfNeedToRedraw()
    {
        foreach (var (widget, gridPos) in _widgets)
        {
            if (widget.Anchor == _grid[gridPos.X, gridPos.Y].Center) continue;

            widget.Clear();
            
            AlignToGrid(widget, gridPos.X, gridPos.Y, _grid[gridPos.X, gridPos.Y].Alignment);
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
            GridAlignment.S => +Coord.Left * (widgetSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
            GridAlignment.SE => Coord.Down * (cellSize.Y / 2 - widgetSize.Y) + Coord.Right * (cellSize.X / 2 - widgetSize.X),
            GridAlignment.SW => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y - cellSize.Y / 2),
            GridAlignment.E => Coord.Right * (cellSize.X / 2 - widgetSize.X) + Coord.Up * (widgetSize.Y / 2),
            GridAlignment.W => Coord.Left * (cellSize.X / 2) + Coord.Up * (widgetSize.Y / 2),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }
}
