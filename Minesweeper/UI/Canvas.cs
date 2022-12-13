using System.Net.NetworkInformation;

namespace Minesweeper.UI;

public class Canvas : Widget
{
    private readonly IRenderable _renderable;
    private readonly PingException
    
    public Canvas(Frame parent, IRenderable renderable) : base(parent)
    {
        _renderable = renderable;
    }

    public override Canvas Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    {
        return base.Grid<Canvas>(row, column, rowSpan, columnSpan, alignment);
    }

    public override Canvas Place(int posX, int posY)
    {
        return base.Grid<Canvas>(posX, posY);
    }

    public override void Render() => _renderable.Render();

    public override void Clear() => _renderable.Clear();

    protected override void Resize() { }
}