namespace Minesweeper;

public static class Grid
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }

    private static bool[,] _bombArray = null!;

    private static Tile[,] _tiles = null!;

    private static Coord _printOffset;

    public static void Generate(short width, short height, bool display = true)
    {
        if (width < 3) width = 3;
        if (height < 3) height = 3;

        Width = width;
        Height = height;
        
        Center();
        
        var random = new Random();
        _tiles = new Tile[width, height];

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            _tiles[x, y] = new Tile();

            if (random.NextDouble() > 0.9) _tiles[x, y].HasBomb = true;
        }

        if (display) Print();
    }

    public static void ClickTile(Coord pos, MouseButtonState buttonState)
    {
        if (!IsInside(pos)) return;
        
        var clickedTile = _tiles[pos.X - _printOffset.X, pos.Y - _printOffset.Y];
        if (clickedTile.Revealed) return;
        
        if (buttonState == MouseButtonState.Left)
        {
            if (clickedTile.Flagged) return;

            clickedTile.Revealed = true;
            
            var tile = Tiles.GetTile(CheckSurrounding(pos));
            Display.Print(pos, tile);
            
            return;
        }

        if (buttonState != MouseButtonState.Right) return;
        
        if (!clickedTile.Flagged)
        {
            Display.Print(pos, Tiles.Flag);
            clickedTile.Flagged = true;
                
            return;
        }
            
        Display.Print(pos, Tiles.Default);
        clickedTile.Flagged = false;
    }

    private static int CheckSurrounding(Coord pos)
    {
        var arrayPos = pos - _printOffset;
        
        if (_tiles[arrayPos.X, arrayPos.Y].HasBomb) return -1;
        
        var bombCount = 0;
        
        for (var x = -1; x < 2; x++)
        for (var y = -1; y < 2; y++)
        {
            var tileX = arrayPos.X + x;
            var tileY = arrayPos.Y + y;
            
            if (tileX < 0 || tileX >= Width || tileY < 0 || tileY >= Height) continue;
            
            if (_tiles[tileX, tileY].HasBomb) bombCount++;
        }

        _tiles[arrayPos.X, arrayPos.Y].NeighbouringBombs = bombCount;
        
        return bombCount;
    }

    public static void Print()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            Display.Print(x + _printOffset.X, y + _printOffset.Y, Tiles.Default);
        }
    }

    private static bool IsInside(Coord pos)
    {
        return pos.X >= _printOffset.X && pos.X < Width + _printOffset.X &&
               pos.Y >= _printOffset.Y && pos.Y < Height + _printOffset.Y;
    }

    private static void Center()
    {
        _printOffset.X = (short) (Display.Width / 2 - Width / 2);
        _printOffset.Y = (short) (Display.Height / 2 - Height / 2);
    }
}
