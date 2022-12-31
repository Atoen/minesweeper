using Minesweeper.ConsoleDisplay;

namespace Minesweeper.UI;

public class Label : VisualElement
{
    public override void Render()
    {
        // if (Text.Animating) Text.Cycle();

        Display.DrawRect(Position, Size, Color);
        
        Content?.Render();

        // Display.Print(Center.X + TextOffset.X, Center.Y + TextOffset.Y, Text.Text, Text.Foreground,
        //     background: Text.Background ?? Color, mode: Text.Mode);
    }

    public override void Clear()
    {
        // var textStart = Center + TextOffset + Coord.Left * (Text.Length / 2);
        //
        // Display.ClearRect(textStart, (Text.Length, 1));

        base.Clear();
    }
}
