using System.Collections.ObjectModel;
using CommunityToolkit.HighPerformance;

namespace Minesweeper.Visual;

public static partial class BigSymbols
{
    public static readonly ReadOnlyDictionary<char, string[]> Symbols;
    
    static BigSymbols()
    {
        Symbols = new ReadOnlyDictionary<char, string[]>(
            new Dictionary<char, string[]>
            {
                { 'A', BigA },
                { 'B', BigB },
                { 'C', BigC },
                { 'D', BigD },
                { 'E', BigE },
                { 'F', BigF },
                { 'G', BigG },
                { 'H', BigH },
                { 'I', BigI },
                { 'J', BigJ },
                { 'K', BigK },
                { 'L', BigL },
                { 'M', BigM },
                { 'N', BigN },
                { 'O', BigO },
                { 'P', BigP },
                { 'Q', BigQ },
                { 'R', BigR },
                { 'S', BigS },
                { 'T', BigT },
                { 'U', BigU },
                { 'V', BigV },
                { 'W', BigW },
                { 'X', BigX },
                { 'Y', BigY },
                { 'Z', BigZ }
            });
    }

    public static readonly string[] BigA = {
        @"    _    ",
        @"   / \   ",
        @"  / _ \  ",
        @" / ___ \ ",
        @"/_/   \_\"
    };
    
    public static readonly string[] BigB = {
        @" ____  ",
        @"| __ ) ",
        @"|  _ \ ",
        @"| |_) |",
        @"|____/ "
    };
    
    public static readonly string[] BigC = {
        @"  ____",
        @" / ___|",
        @"| |  ",
        @"| |___",
        @" \____|"
    };
    
    public static readonly string[] BigD = {
        @" ____  ",
        @"|  _ \ ",
        @"| | | |",
        @"| |_| |",
        @"|____/ "
    };

    public static readonly string[] BigE = {
        @" _____ ",
        @"| ____|",
        @"|  _|  ",
        @"| |___ ",
        @"|_____|"
    };

    public static readonly string[] BigF = {
        @" _____ ",
        @"|  ___|",
        @"| |_   ",
        @"|  _|  ",
        @"|_|    "
    };

    public static readonly string[] BigG = {
        @"  ____ ",
        @" / ___|",
        @"| |  _ ",
        @"| |_| |",
        @" \____|"
    };

    public static readonly string[] BigH = {
        @" _   _ ",
        @"| | | |",
        @"| |_| |",
        @"|  _  |",
        @"|_| |_|"
    };

    public static readonly string[] BigI = {
        @" ___ ",
        @"|_ _|",
        @" | | ",
        @" | | ",
        @"|___|"
    };

    public static readonly string[] BigJ = {
        @"     _ ",
        @"    | |",
        @" _  | |",
        @"| |_| |",
        @" \___/ "
    };

    public static readonly string[] BigK = {
        @" _  __",
        @"| |/ /",
        @"| ' / ",
        @"| . \ ",
        @"|_|\_\"
    };

    public static readonly string[] BigL = {
        @" _     ",
        @"| |    ",
        @"| |    ",
        @"| |___ ",
        @"|_____|"
    };

    public static readonly string[] BigM = {
        @" __  __ ",
        @"|  \/  |",
        @"| |\/| |",
        @"| |  | |",
        @"|_|  |_|"
    };

    public static readonly string[] BigN = {
        @" _   _",
        @"| \ | |",
        @"|  \| |",
        @"| |\  |",
        @"|_| \_|"
    };

    public static readonly string[] BigO = {
        @"  ___  ",
        @" / _ \ ",
        @"| | | |",
        @"| |_| |",
        @" \___/ "
    };

    public static readonly string[] BigP = {
        @" ____  ",
        @"|  _ \ ",
        @"| |_) |",
        @"|  __/ ",
        @"|_|    "
    };

    public static readonly string[] BigQ = {
        @"  ___  ",
        @" / _ \ ",
        @"| | | |",
        @"| |_| |",
        @" \__\_\"
    };
    
    public static readonly string[] BigR = {
        @" ____  ",
        @"|  _ \ ",
        @"| |_) |",
        @"|  _ < ",
        @"|_| \_\"
    };
    
    public static readonly string[] BigS = {
        @" ____  ",
        @"/ ___| ",
        @"\___ \ ",
        @" ___) |",
        @"|____/ "
    };

    public static readonly string[] BigT = {
        @" _____ ",
        @"|_   _|",
        @"  | |  ",
        @"  | |  ",
        @"  |_|  "
    };

    public static readonly string[] BigU = {
        @" _   _ ",
        @"| | | |",
        @"| | | |",
        @"| |_| |",
        @" \___/ "
    };

    public static readonly string[] BigV = {
        @"__     __",
        @"\ \   / /",
        @" \ \ / / ",
        @"  \ V /  ",
        @"   \_/   "
    };

    public static readonly string[] BigW = {
        @"__          __",
        @"\ \        / /",
        @" \ \  /\  / / ",
        @"  \ \/  \/ /  ",
        @"   \__/\__/   "
    };

    public static readonly string[] BigX = {
        @"__  __",
        @"\ \/ /",
        @" \  / ",
        @" /  \ ",
        @"/_/\_\"
    };

    public static readonly string[] BigY = {
        @"__   __",
        @"\ \ / /",
        @" \ V / ",
        @"  | |  ",
        @"  |_|  "
    };

    public static readonly string[] BigZ = {
        @" _____",
        @"|__  /",
        @"  / / ",
        @" / /_ ",
        @"/____|"
    };
}