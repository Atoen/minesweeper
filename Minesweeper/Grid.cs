namespace Minesweeper;

public static class Grid
{
    public static short Width { get; private set; }
    public static short Height { get; private set; }

    private static Tile[,] _tiles = null!;

    private static Coord _printOffset;
    private static bool _firstClick = true;
    private static int _bombs;

    public static void Generate(short bombs, short width, short height, bool display = true)
    {
        if (width < 3) width = 3;
        if (height < 3) height = 3;

        Width = width;
        Height = height;
        _bombs = bombs;
        
        _tiles = new Tile[width, height];
        for (short x = 0; x < Width; x++)
        for (short y = 0; y < Height; y++)
        {
            _tiles[x, y] = new Tile{Pos = {X = x, Y = y}};
        }

        Center();
        
        Input.WindowEvent += delegate
        {
            Center();
            Draw();
        };

        if (display) Draw();
    }

    private static void GenerateBombs(Coord clickPos)
    {
        var nearTiles = new List<Tile>();
        var arrayPos = clickPos - _printOffset;
        
        for (var x = -1; x < 2; x++)
        for (var y = -1; y < 2; y++)
        {
            var tileX = arrayPos.X + x;
            var tileY = arrayPos.Y + y;
            
            if (tileX < 0 || tileX >= Width || tileY < 0 || tileY >= Height) continue;
            
            nearTiles.Add(_tiles[tileX, tileY]);
        }

        var random = new Random();
        var bombSpots = Width * Height - nearTiles.Count;

        if (_bombs > bombSpots) _bombs = bombSpots;
        
        var flatList = new Tile[bombSpots];

        var i = 0;
        for (short x = 0; x < Width; x++)
        for (short y = 0; y < Height; y++)
        {
            if (nearTiles.Contains(_tiles[x, y])) continue;
            
            flatList[i] = _tiles[x, y];
            i++;
        }

        for (i = 0; i < _bombs; i++)
        {
            var j = random.Next(0, bombSpots);
            if (flatList[j].HasBomb)
            {
                i--;
                continue;
            }

            flatList[j].HasBomb = true;
        }

        Parallel.ForEach(flatList.Concat(nearTiles), CheckSurrounding);
    }

    public static void ClickTile(Coord pos, MouseButtonState buttonState)
    {
        if (!IsInside(pos)) return;

        if (_firstClick)
        {
            GenerateBombs(pos);
            _firstClick = false;
        }
        
        var clickedTile = _tiles[pos.X - _printOffset.X, pos.Y - _printOffset.Y];
        if (clickedTile.Revealed) return;
        
        if (buttonState == MouseButtonState.Left)
        {
            if (clickedTile.Flagged) return;

            if (clickedTile.HasBomb)
            {
                DisplayAllBombs();
                return;
            }

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

    private static void CheckSurrounding(Tile tile)
    {
        if (tile.HasBomb)
        {
            tile.NeighbouringBombs = -1;
            return;
        }

        var bombCount = 0;
        
        for (var x = -1; x < 2; x++)
        for (var y = -1; y < 2; y++)
        {
            var tileX = tile.Pos.X + x;
            var tileY = tile.Pos.Y + y;
            
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
        var bombs = 0;
        
        foreach (var tile in _tiles)
        {
            if (tile.HasBomb)
            {
                tile.Revealed = true;
                Display.Draw(tile.Pos + _printOffset, Tiles.Bomb);
                bombs++;
            }
        }

        Console.Title = bombs.ToString();
    }
    
    public static void Draw()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            var tile = _tiles[x, y];

            if (tile.Revealed)
                Display.Draw(x + _printOffset.X, y + _printOffset.Y, Tiles.GetTile(tile.NeighbouringBombs));
            else if (tile.Flagged)
                Display.Draw(x + _printOffset.X, y + _printOffset.Y, Tiles.Flag);
            else
                Display.Draw(x + _printOffset.X, y + _printOffset.Y, Tiles.Default);
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
