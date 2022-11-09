using System.Runtime.InteropServices;
using Minesweeper.Game;
using Minesweeper.UI;

namespace Minesweeper.Display;

public static class Display
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }

    private static bool _refreshing;

    private static readonly List<IRenderable> Renderables = new();
    private static readonly List<IRenderable> AddedRenderables = new();

    private static IRenderer _renderer = null!;

    public static void Init(DisplayMode mode = DisplayMode.Auto)
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        Width = (short) Console.WindowWidth;
        Height = (short) Console.WindowHeight;

        if (mode == DisplayMode.Auto) mode = GetDisplayMode();

        _renderer = mode == DisplayMode.Native
            ? new NativeDisplay(Width, Height)
            : new AnsiDisplay(Width, Height);

        new Thread(Start).Start();
    }

    public static void AddToRenderList(IRenderable renderable) => AddedRenderables.Add(renderable);
    
    public static void Draw(Coord pos, TileDisplay tileDisplay) =>
        _renderer.Draw(pos.X, pos.Y, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);
    
    public static void Draw(int posX, int posY, TileDisplay tileDisplay) =>
        _renderer.Draw(posX, posY, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background) =>
        _renderer.Draw(posX, posY, symbol, foreground, background);

    public static void ClearAt(Coord pos) => _renderer.ClearAt(pos.X, pos.Y);

    public static void ClearAt(int posX, int posY) => _renderer.ClearAt(posX, posY);

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center)
    {
        var startX = alignment switch
        {
            Alignment.Left => posX - text.Length,
            Alignment.Right => posX,
            _ => posX - text.Length / 2
        };
            
        var endX = startX + text.Length;

        for (int x = startX - posX, i = 0; x < endX - posX; x++, i++)
        {
            _renderer.Draw(posX + x, posY, text[i], foreground, background);
        }
    }

    public static void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            _renderer.Draw(pos.X + x, pos.Y + y, symbol, color, color);
        }
    }

    public static void ClearRect(Coord pos, Coord size)
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            _renderer.ClearAt(pos.X + x, pos.Y + y);
        }
    }

    private static void Start()
    {
        const int tickLenght = 1000 / 20;
        var stopwatch = new Stopwatch();

        _refreshing = true;

        while (_refreshing)
        {
            stopwatch.Start();

            Draw();

            stopwatch.Stop();
            var sleepTime = tickLenght - (int) stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();

            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }

    private static void Draw()
    {
        foreach (var renderable in Renderables)
        {
            if (renderable.ShouldRemove)
            {
                renderable.Clear();
                continue;
            }
            
            renderable.Render();
        }
        
        Renderables.RemoveAll(r => r.ShouldRemove);

        if (_renderer.Modified)
        {
            _renderer.Draw();
            _renderer.Modified = false;
        }
        
        if (AddedRenderables.Count == 0) return;

        Renderables.AddRange(AddedRenderables);
        AddedRenderables.Clear();
    }

    private static DisplayMode GetDisplayMode()
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

        var handle = GetStdHandle(-11);
        var mode = 0u;

        GetConsoleMode(handle, ref mode);
        
        return (mode & 0x4) == 0 ? DisplayMode.Native : DisplayMode.Ansi;
    }
}

public enum DisplayMode
{
    Auto,
    Native,
    Ansi
}