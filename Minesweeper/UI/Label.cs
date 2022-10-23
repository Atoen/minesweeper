namespace Minesweeper.UI;

public sealed class Label : Widget
{
    public Label( ConsoleColor color, string text, Alignment alignment = Alignment.Center) : base(color, text, alignment) { }
}