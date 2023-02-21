using Minesweeper.ConsoleDisplay;

namespace Minesweeper;

public static class Application
{
    public static void Start(DisplayMode displayMode = DisplayMode.Auto)
    {
        Display.Init(displayMode);
        Input.Init();
    }

    public static void Exit(Exception? exception = null)
    {
        try
        {
            Input.Stop();
            Display.Stop();
            Display.ResetStyle();
        }
        finally
        {
            Console.Clear();

            if (exception == null) Console.WriteLine(exception);
            else Console.WriteLine("Exiting...");
        }
    }
}