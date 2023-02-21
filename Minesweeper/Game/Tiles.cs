namespace Minesweeper.Game;

public static class Tiles
{
    public static readonly TileDisplay Default = new(Color.Gray, Color.Gray, ' ');
    public static readonly TileDisplay Empty = new(Color.DarkGray, Color.DarkGray, ' ');
    public static readonly TileDisplay Flag = new(Color.Red, Color.Gray, '⚑');
    public static readonly TileDisplay Bomb = new(Color.Black, Color.DarkGray, '\u25D5');

    public static readonly TileDisplay One = new(Color.Blue, Color.DarkGray, '1');
    public static readonly TileDisplay Two = new(Color.Green, Color.DarkGray, '2');
    public static readonly TileDisplay Three = new(Color.Red, Color.DarkGray, '3');
    public static readonly TileDisplay Four = new(Color.DarkBlue, Color.DarkGray, '4');
    public static readonly TileDisplay Five = new(Color.DarkRed, Color.DarkGray, '5');
    public static readonly TileDisplay Six = new(Color.Cyan, Color.DarkGray, '6');
    public static readonly TileDisplay Seven = new(Color.Gray, Color.DarkGray, '7');
    public static readonly TileDisplay Eight = new(Color.Black, Color.DarkGray, '8');

    private static readonly TileDisplay[] RevealedTiles =
    {
        Bomb, Empty, One, Two, Three, Four, Five, Six, Seven, Eight
    };

    public static TileDisplay GetTile(int number) => RevealedTiles[number + 1];
}

public sealed class Tile
{
    public bool Revealed;
    public bool Flagged;
    public bool HasBomb;
    public int NeighbouringBombs;

    public readonly List<Tile> Neighbours = new(8);

    public Vector Pos;

    public void Reset()
    {
        Revealed = false;
        Flagged = false;
        HasBomb = false;
        Neighbours.Clear();
        NeighbouringBombs = 0;
    }
}

public struct TileDisplay
{
    public readonly Color Foreground;
    public readonly Color Background;
    public readonly char Symbol;

    public TileDisplay(Color foreground, Color background, char symbol)
    {
        Foreground = foreground;
        Background = background;
        Symbol = symbol;
    }
}
