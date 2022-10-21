namespace Minesweeper;

public static class Tiles
{
    public static readonly TileDisplay Default = new(ConsoleColor.Gray, ConsoleColor.Gray, ' ');
    public static readonly TileDisplay Empty = new(ConsoleColor.DarkGray, ConsoleColor.DarkGray, ' ');
    public static readonly TileDisplay Flag = new(ConsoleColor.Red, ConsoleColor.Black, '?');
    public static readonly TileDisplay Bomb = new(ConsoleColor.Black, ConsoleColor.Red, '@');

    public static readonly TileDisplay One = new(ConsoleColor.Blue, ConsoleColor.Black, '1');
    public static readonly TileDisplay Two = new(ConsoleColor.Green, ConsoleColor.Black, '2');
    public static readonly TileDisplay Three = new(ConsoleColor.Red, ConsoleColor.Black, '3');
    public static readonly TileDisplay Four = new(ConsoleColor.DarkBlue, ConsoleColor.Black, '4');
    public static readonly TileDisplay Five = new(ConsoleColor.DarkRed, ConsoleColor.Black, '5');
    public static readonly TileDisplay Six = new(ConsoleColor.Cyan, ConsoleColor.Black, '6');
    public static readonly TileDisplay Seven = new(ConsoleColor.Gray, ConsoleColor.Black, '7');
    public static readonly TileDisplay Eight = new(ConsoleColor.Black, ConsoleColor.Black, '8');

    private static readonly TileDisplay ErrorTileDisplay = new(ConsoleColor.Magenta, ConsoleColor.Green, 'E');

    private static readonly TileDisplay[] RevealedTiles =
    {
        Empty, One, Two, Three, Four, Five, Six, Seven, Eight
    };
    
    public static TileDisplay GetTile(int number)
    {
        return number switch
        {
            -1 => Bomb,
            >= 0 and <= 8 => RevealedTiles[number],
            _ => ErrorTileDisplay
        };
    }
}

public sealed class Tile
{
    public bool Revealed;
    public bool Flagged;
    public bool HasBomb;
    public int NeighbouringBombs;

    public short PosX;
    public short PosY;
}

public struct TileDisplay
{
    public readonly ConsoleColor Foreground;
    public readonly ConsoleColor Background;
    public readonly char Symbol;

    public TileDisplay(ConsoleColor foreground, ConsoleColor background, char symbol)
    {
        Foreground = foreground;
        Background = background;
        Symbol = symbol;
    }
}