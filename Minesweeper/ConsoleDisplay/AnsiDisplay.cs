using System.Text;
using Minesweeper.UI.Widgets;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Minesweeper.UI;

namespace Minesweeper.ConsoleDisplay;

public sealed class AnsiDisplay : IRenderer
{ 
    public bool Modified { get; set; }

    private readonly Coord _displaySize;
    private readonly StringBuilder _stringBuilder = new();

    private readonly Pixel[,] _currentPixels;
    private readonly Pixel[,] _lastPixels;
    
    private static readonly ReaderWriterLockSlim LockSlim = new();

    public AnsiDisplay(int width, int height)
    {
        _displaySize.X = (short) width;
        _displaySize.Y = (short) height;

        _currentPixels = new Pixel[width, height];
        _lastPixels = new Pixel[width, height];
    }
    
    public void Draw(int posX, int posY, char symbol, Color fg, Color bg)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;

        _currentPixels[posX, posY].Symbol = symbol;
        _currentPixels[posX, posY].Fg = fg;
        _currentPixels[posX, posY].Bg = bg;
    }

    public void DrawRect(Coord pos, Coord size, Color color, char symbol = ' ')
    {
        var (shouldDraw, start, end) = CalculateDrawArea(pos, size);
        
        if (!shouldDraw) return;

        for (var x = start.X; x < end.X; x++)
        for (var y = start.Y; y < end.Y; y++)
        {
            _currentPixels[x, y].Symbol = symbol;
            _currentPixels[x, y].Fg = Color.Black;
            _currentPixels[x, y].Bg = color;
        }
    }

    public void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode)
    {
        if (posY < 0 || posY >= _displaySize.Y) return;
        
        var startX = alignment switch
        {
            Alignment.Left => posX,
            Alignment.Right => posX - text.Length,
            _ => posX - text.Length / 2
        };
        
        if (startX < 0) startX = 0;
        if (startX >= _displaySize.X) startX = _displaySize.X - 1;
    
        var endX = startX + text.Length;
        
        if (endX >= _displaySize.X) endX = _displaySize.X - 1;

        for (int x = startX - posX, i = 0; x < endX - posX; x++, i++)
        {
            _currentPixels[posX + x, posY].Symbol = text[i];
            _currentPixels[posX + x, posY].Mode = mode;
            _currentPixels[posX + x, posY].Fg = fg;
            _currentPixels[posX + x, posY].Bg = bg;
        }
    }

    public void DrawBuffer(Coord pos, Coord size, Pixel[,] buffer)
    {
        var (shouldDraw, start, end) = CalculateDrawArea(pos, size);
        
        if (!shouldDraw) return;

        for (var x = start.X; x < end.X; x++)
        for (var y = start.Y; y < end.Y; y++)
        {
            _currentPixels[x, y] = buffer[x, y];
        }
    }

    public void DrawBorder(Coord pos, Coord size, Color color, BorderStyle style)
    {
        var (shouldDraw, start, end) = CalculateDrawArea(pos, size);
        
        if (!shouldDraw) return;
        
        
        for (var x = start.X + 1; x < end.X - 1; x++)
        {
            _currentPixels[x, start.Y].Symbol = Border.Symbols[style][BorderFragment.Horizontal];
            _currentPixels[x, start.Y].Fg = color;

            _currentPixels[x, end.Y - 1].Symbol = Border.Symbols[style][BorderFragment.Horizontal];
            _currentPixels[x, end.Y - 1].Fg = color;
        }

        for (var y = start.Y + 1; y < end.Y - 1; y++)
        {
            _currentPixels[start.X, y].Symbol = Border.Symbols[style][BorderFragment.Vertical];
            _currentPixels[start.X, y].Fg = color;
            
            _currentPixels[end.X - 1, y].Symbol = Border.Symbols[style][BorderFragment.Vertical];
            _currentPixels[end.X - 1, y].Fg = color;
        }

        _currentPixels[start.X, start.Y].Symbol = Border.Symbols[style][BorderFragment.UpperLeft];
        _currentPixels[start.X, start.Y].Fg = color;

        var fitsHorizontally = end.X > start.X;
        var fitsVertically = end.Y > start.Y;
        
        if (fitsHorizontally)
        {
            _currentPixels[end.X - 1, start.Y].Symbol = Border.Symbols[style][BorderFragment.UpperRight];
            _currentPixels[end.X - 1, start.Y].Fg = color;
        }
        
        if (fitsVertically)
        {
            _currentPixels[start.X, end.Y - 1].Symbol = Border.Symbols[style][BorderFragment.LowerLeft];
            _currentPixels[start.X, end.Y - 1].Fg = color;
        }

        if (!fitsHorizontally || !fitsVertically) return;
        
        _currentPixels[end.X - 1, end.Y - 1].Symbol = Border.Symbols[style][BorderFragment.LowerRight];
        _currentPixels[end.X - 1, end.Y - 1].Fg = color;
    }

    public void ClearAt(int posX, int posY)
    {
        if (posX < 0 || posX >= _displaySize.X || posY < 0 || posY >= _displaySize.Y) return;
        
        _currentPixels[posX, posY] = Pixel.Cleared;
    }

    public void ClearRect(Coord pos, Coord size)
    {
        var (shouldDraw, start, end) = CalculateDrawArea(pos, size);
        
        if (!shouldDraw) return;

        for (var x = start.X; x < end.X; x++)
        for (var y = start.Y; y < end.Y; y++)
        {
            _currentPixels[x, y] = Pixel.Cleared;
        }
    }

    public void Draw()
    {
        var sw = Stopwatch.StartNew();
    
        CopyToBuffer();

        if (!Modified) return;

        Console.Write(GenerateDisplayString());
        _stringBuilder.Clear();
        
        Modified = false;

        sw.Stop();

        Trace.WriteLine(sw.ElapsedTicks.ToString());
    }

    public void ResetStyle() => Console.Write("\x1b[0m");

    private (bool isNonZero, Coord start, Coord end) CalculateDrawArea(Coord position, Coord size)
    {
        if (position.X >= _displaySize.X || position.Y >= _displaySize.Y)
            return new ValueTuple<bool, Coord, Coord>(false, Coord.Zero, Coord.Zero);

        var start = new Coord
        {
            X = Math.Max(position.X, 0),
            Y = Math.Max(position.Y, 0),
        };
        
        var end = new Coord
        {
            X = Math.Min(position.X + size.X, _displaySize.X),
            Y = Math.Min(position.Y + size.Y, _displaySize.Y),
        };

        return new ValueTuple<bool, Coord, Coord>(true, start, end);
    }

    private void CopyToBuffer()
    {
        for (var x = 0; x < _displaySize.X; x++)
        for (var y = 0; y < _displaySize.Y; y++)
        {
            if (_currentPixels[x, y] == _lastPixels[x, y]) continue;

            Modified = true;
            Array.Copy(_currentPixels, _lastPixels, _displaySize.X * _displaySize.Y);
            
            return;
        }
    }

    private readonly ReadOnlyDictionary<TextMode, string> _ansiTextModes = new(
        new Dictionary<TextMode, string>
        {
            {TextMode.Default, "\x1b[0m"},
            {TextMode.Bold, "\x1b[1m"},
            {TextMode.Italic, "\x1b[3m"},
            {TextMode.Underline, "\x1b[4m"},
            {TextMode.DoubleUnderline, "\x1b[21m"},
            {TextMode.Overline, "\x1b[53m"},
            {TextMode.Strikethrough, "\x1b[9m"}
        });

    private void AppendTextMode(TextMode mode, StringBuilder builder)
    {
        if (mode == TextMode.Default)
        {
            builder.Append(_ansiTextModes[TextMode.Default]);
            
            return;
        }

        foreach (var textMode in _ansiTextModes.Keys)
        {
            if ((mode & textMode) != 0)
            {
                builder.Append(_ansiTextModes[textMode]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private string GenerateDisplayString()
    {
        var lastFg = Color.Transparent;
        var lastBg = Color.Transparent;
        var lastMode = TextMode.Default;

        // starting position for printing the gathered pixel symbols
        var streakStartPos = new Coord();
        var oldStreakPos = new Coord();
        var oldStreakLen = 0;
        var previousIsCleared = false;

        var symbolsBuilder = new StringBuilder();

        _stringBuilder.Append($"\x1b[1;1f");

        for (var y = 0; y < _displaySize.Y; y++)
        for (var x = 0; x < _displaySize.X; x++)
        {
            var pixel = _currentPixels[x, y];

            // Printing the already gathered pixels if next one has different visual properties
            if (pixel.Fg != lastFg || pixel.Bg != lastBg || pixel.Mode != lastMode || (previousIsCleared && pixel.IsEmpty))
            {
                if (symbolsBuilder.Length != 0)
                {
                    // Need to specify new coords for printing
                    if (oldStreakPos.Y != y || oldStreakPos.X + oldStreakLen != streakStartPos.X)
                    {
                        _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
                    }
                    
                    AppendTextMode(lastMode, _stringBuilder);

                    // Resetting the colors to clear the pixels
                    if (previousIsCleared)
                    {
                        _stringBuilder.Append("\x1b[0m");
                    }

                    // Applying the colors for gathered pixels
                    else
                    {
                        lastFg.AppendToBuilder(_stringBuilder);
                        lastBg.AppendToBuilderBg(_stringBuilder);
                    }
                    
                    // Starting new streak of pixels
                    oldStreakLen = symbolsBuilder.Length;
                    oldStreakPos = streakStartPos;
                    
                    _stringBuilder.Append(symbolsBuilder);
                    symbolsBuilder.Clear();
                }
                
                lastMode = pixel.Mode;
                lastFg = pixel.Fg;
                lastBg = pixel.Bg;
            }

            // Setting the start pos of the collected pixel symbols when collecting the first one
            if (symbolsBuilder.Length == 0)
            {
                streakStartPos.X = x;
                streakStartPos.Y = y;
            }

            // Collecting the pixels with same colors together
            if (!pixel.IsEmpty) symbolsBuilder.Append(pixel.Symbol);

            previousIsCleared = pixel.IsCleared;

            // Marking the pixel as empty to not draw it again unnecessarily
            if (pixel.IsCleared) _currentPixels[x, y] = Pixel.Empty;
        }

        // If the screen buffer ends while symbolsBuilder still has unprinted content
        if (symbolsBuilder.Length > 0)
        {
            var lastPixel = _currentPixels[_displaySize.X - 1, _displaySize.Y - 1];

            _stringBuilder.Append($"\x1b[{streakStartPos.Y + 1};{streakStartPos.X + 1}f");
            AppendTextMode(lastMode, _stringBuilder);

            lastPixel.Fg.AppendToBuilder(_stringBuilder);
            lastPixel.Bg.AppendToBuilderBg(_stringBuilder);
            
            _stringBuilder.Append(symbolsBuilder);

            symbolsBuilder.Clear();
        }

        // Resetting the console style after full draw
        _stringBuilder.Append("\x1b[0m");

        Console.Title = _stringBuilder.Length.ToString();

        return _stringBuilder.ToString();
    }
}
