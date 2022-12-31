#nullable enable
using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Button : Widget
{
    public Action? OnClick { get; init; }
    public required IAnimatedText AnimatedText { get; init; }
    public Coord TextOffset = Coord.Zero;

    public Button()
    {
        MouseEventMask = MouseEventMask.MouseMove | MouseEventMask.MouseClick;
    }

    // public override Button Grid(int row, int column, int rowSpan = 1, int columnSpan = 1, GridAlignment alignment = GridAlignment.Center)
    // {
    //     return base.Grid<Button>(row, column, rowSpan, columnSpan, alignment);
    // }
    //
    // public override Button Place(int posX, int posY)
    // {
    //     return base.Grid<Button>(posX, posY);
    // }

    public override void Render()
    {
        // if (Text.Animating && State != State.Disabled) Text.Cycle();
        //
        // Display.DrawRect(Position, Size, Color);
        //
        // Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
        //     background: Text.Background ?? Color, mode: Text.Mode);
    }
    
    protected override void Resize()
    {
        var minSize = new Coord(AnimatedText.Length + 2 * InnerPadding.X, 1 + 2 * InnerPadding.Y);
    
        Size = Size.ExpandTo(minSize);
    }
    
    protected override void OnMouseLeftDown()
    {
        State = State.Pressed;
        OnClick?.Invoke();
    }
}
