using System.Diagnostics;

namespace Minesweeper;

public static class Game
{
    private const int TicksPerSecond = 50;
    private const int TickLenght = 1000 / TicksPerSecond;

    private static bool _isRunning;

    public static void Start()
    {
        if (_isRunning) return;
        _isRunning = true;

        Input.MouseClickEvent += InputOnMouseClickEvent;
        Input.MouseWheelEvent += InputOnMouseWheelEvent;


        Grid.Generate(40, 15);

        MainLoop();
    }

    private static void InputOnMouseWheelEvent(MouseWheelState state)
    {
        Console.Write($"\r{state}");
    }

    private static void InputOnMouseClickEvent(MouseState state)
    {
        if (state.Buttons != 0) Grid.ClickTile(state.Position, state.Buttons);
    }

    public static void Stop()
    {
        Input.Stop();
        _isRunning = false;
    }

    private static void MainLoop()
    {
        var stopwatch = new Stopwatch();

        while (_isRunning)
        {
            stopwatch.Start();

            // Display.Update();
            
            stopwatch.Stop();
            var sleepTime = TickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            
            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }
}
