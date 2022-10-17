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

        Input.Init();
        Input.MouseEvent += InputOnMouseEvent;

        Display.Init(100, 50);
        
        Grid.Generate(100, 50);
        
        MainLoop();
    }

    private static void InputOnMouseEvent(MouseState state)
    {
        if ((state.Buttons & MouseButtonState.Left) != 0)
        {
            Display.Print(state.Position, ' ', ConsoleColor.White, ConsoleColor.White);
        }
    }

    private static void MainLoop()
    {
        var stopwatch = new Stopwatch();

        while (_isRunning)
        {
            stopwatch.Start();

            Display.Update();
            
            stopwatch.Stop();
            var sleepTime = TickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            
            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }
}
