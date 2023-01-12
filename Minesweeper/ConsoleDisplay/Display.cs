using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.ConsoleDisplay;

public static class Display
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    public static DisplayMode Mode { get; private set; }

    private static bool _refreshing;

    private static readonly List<IRenderable> Renderables = new();
    private static readonly List<IRenderable> RemovedRenderables = new();

    private static IRenderer _renderer = null!;

    private static readonly object LockObject = new();

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

    public static void Stop()
    {
        _refreshing = false;
    }

    public static void AddToRenderList(IRenderable renderable)
    {
        lock (LockObject)
        {
            Renderables.Add(renderable);

            // Placing foreground and top renderables later so they don't get covered by background
            Renderables.Sort((r1, r2) => r1.ZIndex.CompareTo(r2.ZIndex));
        }
    }

    public static void RemoveFromRenderList(IRenderable renderable)
    {
        lock (LockObject)
        {
            Renderables.Remove(renderable);
            RemovedRenderables.Add(renderable);
        }
    }

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background)
    {
        _renderer.Draw(posX, posY, symbol, foreground, background);
    }

    public static void ClearAt(Coord pos)
    {
        _renderer.ClearAt(pos.X, pos.Y);
    }

    public static void ClearAt(int posX, int posY)
    {
        _renderer.ClearAt(posX, posY);
    }

    public static void ResetStyle()
    {
        _renderer.ResetStyle();
    }

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center, TextMode mode = TextMode.Default)
    {
        _renderer.Print(posX, posY, text, foreground, background, alignment, mode);
    }

    public static void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        _renderer.DrawRect(pos, size, color, symbol);

        // for (var x = 0; x < size.X; x++)
        // for (var y = 0; y < size.Y; y++)
        // {
        //     _renderer.Draw(pos.X + x, pos.Y + y, symbol, color, color);
        // }
    }

    public static void ClearRect(Coord pos, Coord size)
    {
        _renderer.ClearRect(pos, size);

        // for (var x = 0; x < size.X; x++)
        // for (var y = 0; y < size.Y; y++)
        // {
        //     _renderer.ClearAt(pos.X + x, pos.Y + y);
        // }
    }

    public static void DrawBorder(Coord pos, Coord size, Color color, BorderStyle style)
    {
        _renderer.DrawBorder(pos, size, color, style);
    }

    public static void DrawBuffer(Coord pos, Pixel[,] buffer)
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
        lock (LockObject)
        {
            foreach (var removed in RemovedRenderables) removed.Clear();

            RemovedRenderables.Clear();

            foreach (var renderable in Renderables) renderable.Render();

            _renderer.Draw();
        }
    }

    private static void GetDisplayMode()
    {
        [DllImport("kernel32.dll")]
        static extern nint GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(nint hConsoleInput, out uint mode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(nint handle, uint mode);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Mode = DisplayMode.Native;
            return;
        }

        var handle = GetStdHandle(-11);

        if (handle == (nint) (-1L) || !GetConsoleMode(handle, out var mode))
        {
            Mode = DisplayMode.Native;
            return;
        }

        SetConsoleMode(handle, mode | 4U);
        Mode = DisplayMode.Ansi;
    }
}

public enum DisplayMode
{
    Auto,
    Native,
    Ansi
}
