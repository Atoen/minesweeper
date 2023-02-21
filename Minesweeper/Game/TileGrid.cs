using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;
using Minesweeper.UI.Events;

namespace Minesweeper.Game;

public class TileGrid : Control
{
    public TileGrid(int width, int height, int bombs)
    {
        ShowFocusedBorder = false;

        Width = width;
        Height = height;
        Bombs = bombs;

        _tiles = new Tile[width, height];

        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            _tiles[x, y] = new Tile {Pos = {X = x, Y = y}};
        }

        _tilesLeftToReveal = Width * Height - Bombs;

        _revealed = false;
    }

    public event EventHandler? BombClicked;
    public event EventHandler? ClearedField;
    public event EventHandler<int>? PlacedFlag;
    public event EventHandler<int>? RemovedFlag;

    private readonly Tile[,] _tiles;

    private bool _revealed;
    private int _tilesLeftToReveal;

    public int Bombs { get; private set; }

    public void GenerateNew()
    {
        foreach (var tile in _tiles)
        {
            tile.Reset();
        }

        _tilesLeftToReveal = Width * Height - Bombs;

        _revealed = false;
    }

    private void GenerateBombs(Vector clickPos)
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

        var bombSpots = Width * Height - nearTiles.Count;

        if (Bombs > bombSpots) Bombs = bombSpots;

        var flatList = new Tile[bombSpots];
        var indexList = Enumerable.Range(0, bombSpots).ToList();

        // copying the 2D tiles array into 1D one for easier picking
        var i = 0;
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            if (nearTiles.Contains(_tiles[x, y])) continue;

            flatList[i] = _tiles[x, y];
            i++;
        }

        for (i = 0; i < Bombs; i++)
        {
            // Ensuring that given index is picked only once
            var j = Random.Shared.Next(0, indexList.Count);
            var index = indexList[j];
            indexList.RemoveAt(j);

            flatList[index].HasBomb = true;
        }

        foreach (var tile in _tiles)
        {
            CheckSurrounding(tile);
        }
    }

    private void CheckSurrounding(Tile tile)
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

    private void ShowAllBombs()
    {
        foreach (var tile in _tiles)
        {
            if (!tile.HasBomb) continue;

            tile.Revealed = true;
        }
    }

    private void RevealNearbyTiles(Tile tile)
    {
        var tilesRevealed = 0;

        var tilesToReveal = new List<Tile> {tile};

        // Iterative way of searching for next tiles to reveal
        while (true)
        {
            var newTiles = new List<Tile>();

            foreach (var tileToReveal in tilesToReveal)
            {
                if (tileToReveal.Revealed || tileToReveal.Flagged) continue;

                tileToReveal.Revealed = true;
                tilesRevealed++;

                // If tile is empty then its neighbours are searched too
                if (tileToReveal.NeighbouringBombs == 0) newTiles.AddRange(tileToReveal.Neighbours);
            }

            if (newTiles.Count == 0) break;

            tilesToReveal.Clear();
            tilesToReveal = newTiles;
        }

        _tilesLeftToReveal -= tilesRevealed;

        if (_tilesLeftToReveal < 1)
        {
            ClearedField?.Invoke(this, EventArgs.Empty);
        }
    }

    private void LeftClick(Vector pos)
    {
        var tile = _tiles[pos.X, pos.Y];

        if (tile.Revealed || tile.Flagged) return;

        if (!_revealed)
        {
            GenerateBombs(pos);
            _revealed = true;
        }

        if (tile.HasBomb)
        {
            BombClicked?.Invoke(this, EventArgs.Empty);

            ShowAllBombs();
            return;
        }

        RevealNearbyTiles(tile);
    }

    private void RightClick(Vector pos)
    {
        var tile = _tiles[pos.X, pos.Y];

        if (tile.Revealed) return;

        if (tile.Flagged)
        {
            tile.Flagged = false;
            RemovedFlag?.Invoke(this, -1);
            return;
        }

        tile.Flagged = true;
        PlacedFlag?.Invoke(this, 1);
    }

    protected override void OnMouseLeftDown(MouseEventArgs e)
    {
        LeftClick(e.RelativeCursorPosition);
        base.OnMouseLeftDown(e);
    }

    protected override void OnMouseRightDown(MouseEventArgs e)
    {
        RightClick(e.RelativeCursorPosition);
        base.OnMouseRightDown(e);
    }

    public override void Render()
    {
        var pos = GlobalPosition;

        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            var tile = _tiles[x, y];

            if (tile.Revealed)
                Display.Draw(pos.X + x, pos.Y + y, Tiles.GetTile(tile.NeighbouringBombs));
            else if (tile.Flagged)
                Display.Draw(pos.X + x, pos.Y + y, Tiles.Flag);
            else
                Display.Draw(pos.X + x, pos.Y + y, Tiles.Default);
        }
    }
}