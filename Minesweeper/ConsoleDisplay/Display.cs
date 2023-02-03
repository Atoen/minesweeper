using System.Runtime.InteropServices;
using System.Text;
using Minesweeper.Game;
using Minesweeper.UI;
using Minesweeper.UI.Widgets;

namespace Minesweeper.ConsoleDisplay;

public static partial class Display
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    public static DisplayMode Mode { get; private set; }
    
    private static bool _refreshing;

    private static readonly List<IRenderable> Renderables = new();
    private static readonly List<IRenderable> RemovedRenderables = new();

    private static IRenderer _renderer = null!;

    private static readonly ReaderWriterLockSlim LockSlim = new();

    public static void Init(DisplayMode mode = DisplayMode.Auto)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        Width = Console.WindowWidth;
        Height = Console.WindowHeight;

        Mode = mode;

        if (Mode == DisplayMode.Auto) GetDisplayMode();

        _renderer = Mode == DisplayMode.Ansi
            ? new AnsiDisplay(Width, Height)
            : new NativeDisplay(Width, Height);

        new Thread(Start)
        {
            Name = "Display Thread"
        }.Start();
    }

    public static void Stop() => _refreshing = false;

    public static void AddToRenderList(IRenderable renderable)
    {
        renderable.ZIndexChanged += RenderableOnZIndexChanged;
        
        LockSlim.EnterWriteLock();
        Renderables.Add(renderable);
        LockSlim.ExitWriteLock();
    }

    public static void RemoveFromRenderList(IRenderable renderable)
    {
        renderable.ZIndexChanged -= RenderableOnZIndexChanged;
        
        LockSlim.EnterWriteLock();
        Renderables.Remove(renderable);
        RemovedRenderables.Add(renderable);
        LockSlim.ExitWriteLock();
    }

    public static void Draw(int posX, int posY, char symbol, Color foreground, Color background) => 
        _renderer.Draw(posX, posY, symbol, foreground, background);

    public static void ClearAt(Coord pos) => _renderer.ClearAt(pos.X, pos.Y);

    public static void ClearAt(int posX, int posY) => _renderer.ClearAt(posX, posY);

    public static void ResetStyle() => _renderer.ResetStyle();

    public static void Print(int posX, int posY, string text, Color foreground, Color background,
        Alignment alignment = Alignment.Center, TextMode mode = TextMode.Default) =>
        _renderer.Print(posX, posY, text, foreground, background, alignment, mode);

    public static void Draw(int posX, int posY, TileDisplay tileDisplay) =>
        _renderer.Draw(posX, posY, tileDisplay.Symbol, tileDisplay.Foreground, tileDisplay.Background);

    public static void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        if (!CalculateDrawArea(pos, size, out var start, out var end)) return;
        
        _renderer.DrawRect(start, end, color, symbol);
    }

    public static void DrawLine(Coord pos, Coord direction, int length, Color foreground, Color background, char symbol)
    {
        if (!CalculateLineLength(pos, direction, length, out var start, out var calculatedLenght)) return;
        
        _renderer.DrawLine(start, direction, calculatedLenght, foreground, background, symbol);
    }

    public static void ClearRect(Coord pos, Coord size)
    {
        if (!CalculateDrawArea(pos, size, out var start, out var end)) return;
        
        _renderer.ClearRect(start, end);
    }

    public static void DrawBorder(Coord pos, Coord size, Color color, BorderStyle style)
    {
        if (!CalculateDrawArea(pos, size, out var start, out var end)) return;
        
        _renderer.DrawBorder(start, end, color, style);
    }

    public static void DrawBuffer(Coord pos, Pixel[,] buffer)
    {
        var size = new Coord {X = buffer.GetLength(0), Y = buffer.GetLength(1)};

        if (!CalculateDrawArea(pos, size, out var start, out var end)) return;
        
        _renderer.DrawBuffer(start, end, buffer);
    }

    private static void Start()
    {
        const int tickLength = 1000 / 20;
        var stopwatch = new Stopwatch();

        _refreshing = true;

        while (_refreshing)
        {
            stopwatch.Start();

            Draw();

            stopwatch.Stop();
            var sleepTime = tickLength - (int) stopwatch.ElapsedMilliseconds;
            
            stopwatch.Reset();

            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
    }
    
    private static bool CalculateDrawArea(Coord position, Coord size, out Coord start, out Coord end)
    {
        start = new Coord();
        end = new Coord();
        
        if (position.X >= Width || position.Y >= Height || size == Coord.Zero) return false;
        
        start.X = Math.Max(position.X, 0);
        start.Y = Math.Max(position.Y, 0);
        
        end.X = Math.Min(Math.Max(position.X + size.X, 0), Width);
        end.Y = Math.Min(Math.Max(position.Y + size.Y, 0), Height);

        return start.X != end.X && start.Y != end.Y;
    }

    private static bool CalculateLineLength(Coord position, Coord direction, int originalLength,
        out Coord start, out int calculatedLenght)
    {
        start = new Coord();
        calculatedLenght = 0;

        if (originalLength == 0 || direction == Coord.Zero) return false;
        
        start.X = Math.Max(Math.Min(position.X, Width - 1), 0);
        start.Y = Math.Max(Math.Min(position.Y, Height - 1), 0);

        // Skipping unnecessary operations if line is not diagonal
        if (direction is {X: not 0, Y: not 0})
        {
            if (position.X < 0 && direction.X <= 0) return false;
            if (position.X >= Width && direction.X >= 0) return false;
            if (position.Y < 0 && direction.Y <= 0) return false;
            if (position.Y >= Height && direction.Y >= 0) return false;
            
            var adjusted = false;

            if (start.X != position.X)
            {
                var difference = position.X - start.X;
                if (Math.Abs(difference) > originalLength) return false;
                
                start.Y += direction.Y * difference;
                start.Y = Math.Max(Math.Min(start.Y, Height - 1), 0);

                adjusted = true;
            }
            
            if (start.Y != position.Y && !adjusted)
            {
                var difference = position.Y - start.Y;
                if (Math.Abs(difference) > originalLength) return false;
                
                start.X += direction.X * difference;
                start.X = Math.Max(Math.Min(start.X, Width - 1), 0);
            }
        }
        
        var end = new Coord
        {
            X = Math.Min(Math.Max(position.X + direction.X * originalLength, 0), Width - 1),
            Y = Math.Min(Math.Max(position.Y + direction.Y * originalLength, 0), Height - 1)
        };
        
        var lengthX = Math.Abs(end.X - start.X);
        var lengthY = Math.Abs(end.Y - start.Y);

        calculatedLenght = direction switch
        {
            {X: not 0, Y: 0} => lengthX,
            {X: 0, Y: not 0} => lengthY,
            _ => Math.Min(lengthX, lengthY)
        };

        return calculatedLenght > 0;
    }
    
    private static void RenderableOnZIndexChanged(Component sender, ZIndexChangedEventArgs e) => SortRenderables();

    private static void SortRenderables()
    {
        LockSlim.EnterWriteLock();
        Renderables.Sort((r1, r2) => r1.CompareTo(r2));
        LockSlim.ExitWriteLock();
    }

    private static void Draw()
    {
        LockSlim.EnterWriteLock();
        
        foreach (var removed in RemovedRenderables) removed.Clear();

        RemovedRenderables.Clear();

        foreach (var renderable in Renderables)
        {
            if (renderable.ShouldRender) renderable.Render();
        }

        LockSlim.ExitWriteLock();
        
        _renderer.Draw();
    }

    private static void GetDisplayMode()
    {
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
    
    [LibraryImport("kernel32.dll")]
    private static partial nint GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleInput, out uint mode);

    [LibraryImport("kernel32.dll")]
    private static partial void SetConsoleMode(nint handle, uint mode);
}

public enum DisplayMode
{
    Auto,
    Native,
    Ansi
}
