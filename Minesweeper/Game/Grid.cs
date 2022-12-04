namespace Minesweeper.Game;

public static class Grid
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    private static Tile[,] _tiles = null!;
    
    private static bool _firstClick = true;
    private static int _bombs;

    public static void Generate(int bombs, int width, int height)
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
        
        Draw();
    }

    private static void GenerateBombs(Coord clickPos)
    {
        // Tiles near the clicked one are guaranteed not to have bombs
        var nearTiles = new List<Tile>();

        // Gathering near tiles
        for (var x = -1; x < 2; x++)
        for (var y = -1; y < 2; y++)
        {
            var tileX = clickPos.X + x;
            var tileY = clickPos.Y + y;
            
            if (tileX < 0 || tileX >= Width || tileY < 0 || tileY >= Height) continue;
            
            nearTiles.Add(_tiles[tileX, tileY]);
        }

        var random = new Random();
        var bombSpots = Width * Height - nearTiles.Count;

        if (_bombs > bombSpots) _bombs = bombSpots;
        
        var flatList = new Tile[bombSpots];
        var indexList = Enumerable.Range(0, bombSpots).ToList();

        // copying the tiles 2D array into 1D one for easier picking
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
            // Ensuring that given index is picked only once
            var j = random.Next(0, indexList.Count);
            var index = indexList[j];
            indexList.RemoveAt(j);
        
            flatList[index].HasBomb = true;
        }

        foreach (var tile in flatList.Concat(nearTiles))
        {
            CheckSurrounding(tile);
        }
    }

    public static void ClickTile(Coord pos, MouseButtonState buttonState)
    {
        if (!IsInside(pos)) return;

        if (_firstClick)
        {
            GenerateBombs(pos);
            _firstClick = false;
        }
        
        var clickedTile = _tiles[pos.X, pos.Y];
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
            Display.Display.Draw(pos, Tiles.Flag);
            clickedTile.Flagged = true;
                
            return;
        }
            
        Display.Display.Draw(pos, Tiles.Default);
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
                
                Display.Display.Draw(tileToReveal.Pos, Tiles.GetTile(tileToReveal.NeighbouringBombs));
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
            if (!tile.HasBomb) continue;
            
            tile.Revealed = true;
            Display.Display.Draw(tile.Pos, Tiles.Bomb);
        }
        
    }
    
    public static void Draw()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            var tile = _tiles[x, y];

            if (tile.Revealed)
                Display.Display.Draw(x, y, Tiles.GetTile(tile.NeighbouringBombs));
            else if (tile.Flagged)
                Display.Display.Draw(x, y, Tiles.Flag);
            else
                Display.Display.Draw(x, y, Tiles.Default);
        }
    }

    private static bool IsInside(Coord pos)
    {
        return pos.X >= 0 && pos.X < Width &&
               pos.Y >= 0 && pos.Y < Height;
    }
}
