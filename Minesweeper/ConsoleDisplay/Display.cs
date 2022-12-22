using System.Runtime.InteropServices;
using System.Text;
using Minesweeper.Game;
using Minesweeper.UI;

namespace Minesweeper.ConsoleDisplay;

public static class Display
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    
    public static DisplayMode Mode { get; private set; }

    private static bool _refreshing;

    private static readonly List<IRenderable> Renderables = new();
    private static readonly List<IRenderable> AddedRenderables = new();

    private static IRenderer _renderer = null!;

    public static void Init(DisplayMode mode = DisplayMode.Auto)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
        
        Width = Console.WindowWidth;
        Height = Console.WindowHeight;

        Mode = mode;

        if (Mode == DisplayMode.Auto) GetDisplayMode();

        _renderer = Mode == DisplayMode.Native
            ? new NativeDisplay(Width, Height)
            : new AnsiDisplay(Width, Height);

        new Thread(Start)
        {
            Name = "Display Thread"
        }.Start();
    }

    public static void Stop() => _refreshing = false;
    
    public static void AddToRenderList(IRenderable renderable) => AddedRenderables.Add(renderable);

    public static void Draw(Coord pos, TileDisplay tileDisplay) =>
        _renderer.Draw(pos.X, pos.Y, tileDisplay);

    public static void Draw(int posX, int posY, TileDisplay tileDisplay) =>
        _renderer.Draw(posX, posY, tileDisplay);

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background) =>
        _renderer.Draw(posX, posY, symbol, foreground, background);

    public static void ClearAt(Coord pos) => _renderer.ClearAt(pos.X, pos.Y);

    public static void ClearAt(int posX, int posY) => _renderer.ClearAt(posX, posY);

    public static void ResetStyle() => _renderer.ResetStyle();

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center, TextMode mode = TextMode.Default)
    {
        _renderer.Print(posX, posY, text, foreground, background, alignment, mode);
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

    public static void DrawBuffer(Coord pos, AnsiDisplay.Pixel[,] buffer)
    {
        var size = new Coord(buffer.GetLength(0), buffer.GetLength(1));
        
        _renderer.DrawBuffer(pos, size, buffer);
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
        
        _renderer.Draw();
        
        if (AddedRenderables.Count == 0) return;

        Renderables.AddRange(AddedRenderables);
        AddedRenderables.Clear();

        // Placing foreground and top renderables later so they don't get covered by background
        Renderables.Sort((r1, r2) => r1.Layer.CompareTo(r2.Layer));
    }

    private static void GetDisplayMode()
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

        var handle = GetStdHandle(-11);
        var mode = 0u;

        GetConsoleMode(handle, ref mode);

        const int virtualTerminalProcessing = 0x4;
        Mode = (mode & virtualTerminalProcessing) == 0 ? DisplayMode.Native : DisplayMode.Ansi;
    }
}

public enum DisplayMode
{
    Auto,
    Native,
    Ansi
}

[Flags]
public enum TextMode
{
    Default = 0,
    Bold = 1,
    Underline = 1 << 1,
    Italic = 1 << 2,
    Strikethrough = 1 << 3
}

public enum Layer
{
    Background = 0,
    Foreground = 1,
    Top = 2
}