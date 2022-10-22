using System.Runtime.CompilerServices;
using Minesweeper;
using Minesweeper.UI;

namespace Minesweeper;

public static class MainMenu
{
    private static short GridWidth;
    private static short GridHeight;
    private static short BombAmount;

    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static readonly Task WaitingTask = Task.Delay(Timeout.Infinite, CancellationTokenSource.Token);

    private static Button button;

    public static void Display()
    {
        var button2 = new Button("PLAY", Alignment.Center)
        {
            Pos = new Coord(5, 8),
            Size = new Coord(10, 4),
            DefaultColor = ConsoleColor.Blue,
            HighlightedColor = ConsoleColor.Cyan,
            PressedColor = ConsoleColor.White
        };

        button2.ClickAction = () => { button2.Text = "Gaming"; };
        
        button = new Button("PLAY", Alignment.Center)
        {
            Pos = new Coord(5, 1),
            Size = new Coord(10, 4),
            DefaultColor = ConsoleColor.Blue,
            HighlightedColor = ConsoleColor.Cyan,
            PressedColor = ConsoleColor.White,
            ClickAction = () =>
            {
                button2.Text = "Najtotalniejsze ORO";
            }
        };
        
        var button3 = new Button("PLAY", Alignment.Right)
        {
            Pos = new Coord(5, 15),
            Size = new Coord(10, 4),
            DefaultColor = ConsoleColor.Blue,
            HighlightedColor = ConsoleColor.Cyan,
            PressedColor = ConsoleColor.White
        };
    }

    // private static void InputOnMouseEvent(MouseState state)
    // {
    //     if (button.IsOver(state.Position))
    //     {
    //         if ((state.Buttons & MouseButtonState.Left) != 0)
    //         {
    //             button.ClickAction?.Invoke();
    //             button.State = ButtonState.Pressed;
    //         }
    //
    //         else
    //         {
    //             button.State = ButtonState.Highlighted;
    //         }
    //
    //     }
    //     else
    //         button.State = ButtonState.Default;
    //     
    // }
}
    