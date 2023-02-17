using System.Reflection;
using System.Text;

namespace Minesweeper.Visual.FigletText;

public class FigletFont
{
    private static FigletFont? _defaultFont;
    private static FigletFont? _fourMax;
    private static FigletFont? _amc3;
    private static FigletFont? _ansiRegular;
    private static FigletFont? _ansiShadow;
    private static FigletFont? _bigFig;
    private static FigletFont? _calvinS;
    public static FigletFont Default { get; } = _defaultFont ??= LoadBuiltInFont(BuiltInFigletFonts.Default);
    public static FigletFont FourMax { get; } = _fourMax ??= LoadBuiltInFont(BuiltInFigletFonts.FourMax);
    public static FigletFont Amc3 { get; } = _amc3 ??= LoadBuiltInFont(BuiltInFigletFonts.Amc3);
    public static FigletFont AnsiRegular { get; } = _ansiRegular ??= LoadBuiltInFont(BuiltInFigletFonts.AnsiRegular);
    public static FigletFont AnsiShadow { get; } = _ansiShadow ??= LoadBuiltInFont(BuiltInFigletFonts.AnsiShadow);
    public static FigletFont BigFig { get; } = _bigFig ??= LoadBuiltInFont(BuiltInFigletFonts.BigFig);
    public static FigletFont CalvinS { get; } = _calvinS ??= LoadBuiltInFont(BuiltInFigletFonts.CalvinS);

    private const string FontFileSignature = "flf2a";
    public required char HardBlank { get; init;}
    public required int Height { get; init;}
    public required int BaseLine { get; init;}
    public required int MaxLength { get; init;}
    public required int OldLayout { get; init;}
    public required int CommentLines { get; init;}
    public required string[][] Lines { get; init;}
    public required string Commit { get; init;}
    
    public string GetCharacterLine(char symbol, int line)
    {
        if (line < 0 || line >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(line));
        }

        return Lines[symbol][line];
    }

    private static FigletFont LoadBuiltInFont(BuiltInFigletFonts builtInFont)
    {
        var fileName = FontFileName(builtInFont);
        var resourceName = $"{typeof(FigletFont).GetTypeInfo().Namespace}.{fileName}.flf";
        
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"{builtInFont} font resource file not found.");
        }

        return LoadFont(stream);
    }

    private static string FontFileName(BuiltInFigletFonts builtInFont) => builtInFont switch
    {
        BuiltInFigletFonts.Default => "default",
        BuiltInFigletFonts.FourMax => "4max",
        BuiltInFigletFonts.Amc3 => "amc3",
        BuiltInFigletFonts.AnsiRegular => "ansi regular",
        BuiltInFigletFonts.AnsiShadow => "ansi shadow",
        BuiltInFigletFonts.BigFig => "bigfig",
        BuiltInFigletFonts.CalvinS => "calvin s",
        _ => throw new ArgumentOutOfRangeException(nameof(builtInFont), builtInFont, null)
    };

    public static FigletFont LoadFont(Stream stream)
    {
        using var reader = new StreamReader(stream);

        var config = reader.ReadLine()?.Split(' ');

        if (config == null || !config[0].StartsWith(FontFileSignature))
        {
            throw new FileLoadException("Font file is missing the signature");
        }

        var hardBlank = config[0].Last();
        var height = int.Parse(config[1]);
        var baseLine = int.Parse(config[2]);
        var maxLength = int.Parse(config[3]);
        var oldLayout = int.Parse(config[4]);
        var commentLines = int.Parse(config[5]);

        var builder = new StringBuilder();
        for (var line = 0; line < commentLines; line++)
        {
            builder.AppendLine(reader.ReadLine());
        }

        var commit = builder.ToString();

        var lines = new string[256][];

        var currentChar = 32;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? string.Empty;

            if (int.TryParse(line, out var charIndex))
            {
                currentChar = charIndex;
            }

            lines[currentChar] = new string[height];

            var lineIndex = 0;
            while (lineIndex < height)
            {
                lines[currentChar][lineIndex] = line.TrimEnd('@').Replace(hardBlank, ' ');
                
                if (line.EndsWith("@@")) break;

                line = reader.ReadLine() ?? string.Empty;
                lineIndex++;
            }

            currentChar++;
        }

        return new FigletFont
        {
            Height = height,
            HardBlank = hardBlank,
            Commit = commit,
            Lines = lines,
            BaseLine = baseLine,
            CommentLines = commentLines,
            MaxLength = maxLength,
            OldLayout = oldLayout,
        };
    }
}

public enum BuiltInFigletFonts
{
    Default,
    FourMax,
    Amc3,
    AnsiRegular,
    AnsiShadow,
    BigFig,
    CalvinS
}