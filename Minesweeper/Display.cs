using System.Runtime.InteropServices;
using Minesweeper.UI;

namespace Minesweeper;

public static class Display
{
    private static bool _refreshing;

    private static IDisplayProvider _displayProvider = null!;

    public static short Width { get; private set; }
    public static short Height { get; private set; }

    public static void Init(int option)
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        Width = (short) Console.WindowWidth;
        Height = (short) Console.WindowHeight;

        if (option == 0)
        {
            Console.Title = "Native Display";
            _displayProvider = new NativeDisplay(Width, Height);
        } 
        else if (option == 1)
        {
            Console.Title = "Ansi Display";
            _displayProvider = new AnsiDisplay(Width, Height);
        }
        
        new Thread(Start).Start();
    }

    public static void AddToRenderList(IRenderable renderable) => _displayProvider.AddToRenderList(renderable);
    
    public static void Draw(Coord pos, TileDisplay tileDisplay) =>
        _displayProvider.Draw(pos.X, pos.Y, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void Draw(int posX, int posY, TileDisplay tileDisplay) =>
        _displayProvider.Draw(posX, posY, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background) =>
        _displayProvider.Draw(posX, posY, symbol, foreground, background);

    public static void ClearAt(Coord pos) => _displayProvider.ClearAt(pos.X, pos.Y);

    public static void ClearAt(int posX, int posY) => _displayProvider.ClearAt(posX, posY);

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center) =>
        _displayProvider.Print(posX, posY, text, foreground, background, alignment);

    public static void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ') =>
        _displayProvider.DrawRect(pos, size, color, symbol);

    public static void ClearRect(Coord pos, Coord size) => _displayProvider.ClearRect(pos, size);

    private static void Start()
    {
        var tickLenght = 1000 / 20;
        var stopwatch = new Stopwatch();

        _refreshing = true;

        while (_refreshing)
        {
            stopwatch.Start();

            _displayProvider.Draw();

            stopwatch.Stop();
            var sleepTime = tickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();

            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }

    public static void Stop() => _refreshing = false;

    public static void SetSize(int width, int height)
    {
        
    }
}