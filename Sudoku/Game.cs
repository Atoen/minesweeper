using System.Diagnostics;

namespace Sudoku;

public static class Game
{
    private const int TicksPerSecond = 20;
    private const int TickLenght = 1000 / TicksPerSecond;

    private static bool _isRunning;

    public static void Start()
    {
        if (_isRunning) return;
        _isRunning = true;

        Input.Init();
        Input.MouseEvent += ConsoleListenerOnMouseEvent;
        Input.KeyEvent += ConsoleListenerOnKeyEvent;

        Display.Init();
        // MainLoop();
    }

    private static void ConsoleListenerOnKeyEvent(KeyboardState r)
    {
        Console.Write($"{r.Char} {r.Pressed}");
    }

    private static void ConsoleListenerOnMouseEvent(MouseState state)
    {
        Console.Write($"\r{state.Position} {state.Buttons:F} {state.Flags:F}\t\t");
    }

    private static void MainLoop()
    {
        var stopwatch = new Stopwatch();

        while (_isRunning)
        {
            stopwatch.Start();

            stopwatch.Stop();

            var sleepTime = TickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            
            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }
}
