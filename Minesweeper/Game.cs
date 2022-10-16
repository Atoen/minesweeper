using System.Diagnostics;

namespace Minesweeper;

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
        Input.MouseEvent += InputOnMouseEvent;

        Display.Init(1, 0);
        
        Grid.Generate(100, 50);
        
        MainLoop();
    }

    private static void InputOnMouseEvent(MouseState state)
    {
        if (state.Buttons.HasFlag(MouseButtonState.Left))
        {
            Display.ClearAt(state.Position.X, state.Position.Y);
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
