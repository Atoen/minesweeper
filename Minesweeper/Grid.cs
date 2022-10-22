namespace Minesweeper;

public static class Grid
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }

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

        for (short x = 0; x < width; x++)
        for (short y = 0; y < height; y++)
        {
            _tiles[x, y] = new Tile {Pos = {X = x, Y = y}};

            if (random.NextDouble() > 0.8) _tiles[x, y].HasBomb = true;
        }

        foreach (var tile in _tiles)
        {
            CheckSurrounding(tile.Pos);
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

            if (clickedTile.HasBomb)
            {
                DisplayAllBombs();
                return;
            };

            RevealNearbyTiles(clickedTile);
            
            return;
        }

        if (buttonState != MouseButtonState.Right) return;
        
        if (!clickedTile.Flagged)
        {
            Display.Draw(pos, Tiles.Flag);
            clickedTile.Flagged = true;
                
            return;
        }
            
        Display.Draw(pos, Tiles.Default);
        clickedTile.Flagged = false;
    }

    private static void CheckSurrounding(Coord arrayPos)
    {
        var tile = GetTile(arrayPos);
        if (tile.HasBomb) return;

        var bombCount = 0;
        
        for (var x = -1; x < 2; x++)
        for (var y = -1; y < 2; y++)
        {
            var tileX = arrayPos.X + x;
            var tileY = arrayPos.Y + y;
            
            if (tileX < 0 || tileX >= Width || tileY < 0 || tileY >= Height) continue;
            
            if (x != 0 || y != 0) tile.Neighbours.Add(_tiles[tileX, tileY]);
            
            if (_tiles[tileX, tileY].HasBomb) bombCount++;
        }

        tile.NeighbouringBombs = bombCount;
    }

    private static void RevealNearbyTiles(Tile tile)
    {
        var tilesToReveal = new List<Tile> {tile};

        // Iterative way of searching for next tiles to reveal
        while (true)
        {
            var newTiles = new List<Tile>();

            foreach (var tileToReveal in tilesToReveal)
            {
                if (tileToReveal.HasBomb || tileToReveal.Flagged || tileToReveal.Revealed) continue;

                tileToReveal.Revealed = true;
                
                // If tile is empty then its neighbours are searched too 
                if (tileToReveal.NeighbouringBombs == 0) newTiles.AddRange(tileToReveal.Neighbours);
                
                Display.Draw(tileToReveal.Pos + _printOffset, Tiles.GetTile(tileToReveal.NeighbouringBombs));
            }

            if (newTiles.Count == 0) break;
            
            tilesToReveal.Clear();
            tilesToReveal = newTiles;
        }
    }

    private static void DisplayAllBombs()
    {
        foreach (var tile in _tiles)
        {
            if (tile.HasBomb) Display.Draw(tile.Pos + _printOffset, Tiles.Bomb);
        }
    }
    
    public static void Print()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            Display.Draw(x + _printOffset.X, y + _printOffset.Y, Tiles.Default);
        }
    }

    private static Tile GetTile(Coord pos) => _tiles[pos.X, pos.Y];

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
