using System.Runtime.InteropServices;
using System.Text;
using Minesweeper.Game;
using Minesweeper.UI;

namespace Minesweeper.Display;

public static class Display
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    
    public static DisplayMode Mode { get; private set; }

    private static bool _refreshing;
    private static bool _rendering;

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

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background, Layer layer = Layer.Foreground) =>
        _renderer.Draw(posX, posY, symbol, foreground, background, layer);

    public static void ClearAt(Coord pos, Layer layer = Layer.Foreground) => _renderer.ClearAt(pos.X, pos.Y, layer);

    public static void ClearAt(int posX, int posY, Layer layer = Layer.Foreground) => _renderer.ClearAt(posX, posY, layer);

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center, Layer layer = Layer.Foreground)
    {
        var startX = alignment switch
        {
            Alignment.Left => posX,
            Alignment.Right => posX - text.Length,
            _ => posX - text.Length / 2
        };
            
        var endX = startX + text.Length;

        for (int x = startX - posX, i = 0; x < endX - posX; x++, i++)
        {
            _renderer.Draw(posX + x, posY, text[i], foreground, background, layer);
        }
    }

    public static void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ', Layer layer = Layer.Foreground)
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            _renderer.Draw(pos.X + x, pos.Y + y, symbol, color, color, layer);
        }
    }

    public static void ClearRect(Coord pos, Coord size, Layer layer = Layer.Foreground)
    {
        for (var x = 0; x < size.X; x++)
        for (var y = 0; y < size.Y; y++)
        {
            _renderer.ClearAt(pos.X + x, pos.Y + y, layer);
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

            _rendering = true;
            
            Draw();
            
            _rendering = false;

            stopwatch.Stop();
            var sleepTime = tickLenght - (int) stopwatch.ElapsedMilliseconds;

            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            
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

        Mode = (mode & 0x4) == 0 ? DisplayMode.Native : DisplayMode.Ansi;
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
    Default,
    Bold,
    Underline,
    Italic,
    Strikethrough
}

public enum Layer
{
    Background = 0,
    Foreground = 1,
    Top = 2
}