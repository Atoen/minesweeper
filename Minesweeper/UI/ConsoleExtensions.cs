using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Minesweeper.UI;

/// <summary>
/// Controls colored console output by <see langword="Pastel"/>.
/// </summary>
public static class ConsoleExtensions
{
    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);
    
    private static bool _enabled;

    private delegate string ColorFormat(string input, Color color);

    private delegate string HexColorFormat(string input, string hexColor);

    private enum ColorPlane : byte
    {
        Foreground,
        Background
    }

    private const string FormatStringStart = "\x1b[{0};2;";
    private const string FormatStringColor = "{1};{2};{3}m";
    private const string FormatStringContent = "{4}";
    private const string FormatStringEnd = "\x1b[0m";

    private static readonly string _formatStringFull =
        $"{FormatStringStart}{FormatStringColor}{FormatStringContent}{FormatStringEnd}";
    
    private static readonly ReadOnlyDictionary<ColorPlane, string> PlaneFormatModifiers = new(
        new Dictionary<ColorPlane, string>
        {
            [ColorPlane.Foreground] = "38",
            [ColorPlane.Background] = "48"
        });

    private static readonly Regex CloseNestedPastelStringRegex1 =
        new($"({FormatStringEnd.Replace("[", @"\[")})+", RegexOptions.Compiled);

    private static readonly Regex CloseNestedPastelStringRegex2 = new(
        $"(?<!^)(?<!{FormatStringEnd.Replace("[", @"\[")})(?<!{string.Format($"{FormatStringStart.Replace("[", @"\[")}{FormatStringColor}", new[] {$"(?:{PlaneFormatModifiers[ColorPlane.Foreground]}|{PlaneFormatModifiers[ColorPlane.Background]})"}.Concat(Enumerable.Repeat(@"\d{1,3}", 3)).Cast<object>().ToArray())})(?:{string.Format(FormatStringStart.Replace("[", @"\["), $"(?:{PlaneFormatModifiers[ColorPlane.Foreground]}|{PlaneFormatModifiers[ColorPlane.Background]})")})",
        RegexOptions.Compiled);

    private static readonly ReadOnlyDictionary<ColorPlane, Regex> CloseNestedPastelStringRegex3 = new(
        new Dictionary<ColorPlane, Regex>
        {
            [ColorPlane.Foreground] =
                new(
                    $"(?:{FormatStringEnd.Replace("[", @"\[")})(?!{string.Format(FormatStringStart.Replace("[", @"\["), PlaneFormatModifiers[ColorPlane.Foreground])})(?!$)",
                    RegexOptions.Compiled),
            [ColorPlane.Background] =
                new(
                    $"(?:{FormatStringEnd.Replace("[", @"\[")})(?!{string.Format(FormatStringStart.Replace("[", @"\["), PlaneFormatModifiers[ColorPlane.Background])})(?!$)",
                    RegexOptions.Compiled)
        });


    private static readonly Func<string, int> ParseHexColor =
        hc => int.Parse(hc.Replace("#", ""), NumberStyles.HexNumber);

    private static readonly Func<string, Color, ColorPlane, string> _colorFormat = (i, c, p) =>
        string.Format(_formatStringFull, PlaneFormatModifiers[p], c.R, c.G, c.B, CloseNestedPastelStrings(i, c, p));

    private static readonly Func<string, string, ColorPlane, string> ColorHexFormat =
        (i, c, p) => _colorFormat(i, Color.FromArgb(ParseHexColor(c)), p);

    private static readonly ColorFormat NoColorOutputFormat = (i, _) => i;
    private static readonly HexColorFormat NoHexColorOutputFormat = (i, _) => i;

    private static readonly ColorFormat ForegroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Foreground);

    private static readonly HexColorFormat ForegroundHexColorFormat =
        (i, c) => ColorHexFormat(i, c, ColorPlane.Foreground);

    private static readonly ColorFormat BackgroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Background);

    private static readonly HexColorFormat BackgroundHexColorFormat =
        (i, c) => ColorHexFormat(i, c, ColorPlane.Background);


    private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>> ColorFormatFuncs =
        new(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>>
        {
            [false] = new(new Dictionary<ColorPlane, ColorFormat>
            {
                [ColorPlane.Foreground] = NoColorOutputFormat,
                [ColorPlane.Background] = NoColorOutputFormat
            }),
            [true] = new(new Dictionary<ColorPlane, ColorFormat>
            {
                [ColorPlane.Foreground] = ForegroundColorFormat,
                [ColorPlane.Background] = BackgroundColorFormat
            })
        });

    private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>>
        HexColorFormatFuncs = new(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>>
        {
            [false] = new(new Dictionary<ColorPlane, HexColorFormat>
            {
                [ColorPlane.Foreground] = NoHexColorOutputFormat,
                [ColorPlane.Background] = NoHexColorOutputFormat
            }),
            [true] = new(new Dictionary<ColorPlane, HexColorFormat>
            {
                [ColorPlane.Foreground] = ForegroundHexColorFormat,
                [ColorPlane.Background] = BackgroundHexColorFormat
            })
        });


    static ConsoleExtensions()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var iStdOut = GetStdHandle(StdOutputHandle);

            var enable = GetConsoleMode(iStdOut, out var outConsoleMode)
                         && SetConsoleMode(iStdOut, outConsoleMode | EnableVirtualTerminalProcessing);
        }


        if (Environment.GetEnvironmentVariable("NO_COLOR") == null)
            Enable();
        else
            Disable();
    }


    /// <summary>
    ///     Enables any future console color output produced by Pastel.
    /// </summary>
    public static void Enable()
    {
        _enabled = true;
    }

    /// <summary>
    ///     Disables any future console color output produced by Pastel.
    /// </summary>
    public static void Disable()
    {
        _enabled = false;
    }


    /// <summary>
    ///     Returns a string wrapped in an ANSI foreground color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string Pastel(this string input, Color color)
    {
        return ColorFormatFuncs[_enabled][ColorPlane.Foreground](input, color);
    }

    /// <summary>
    ///     Returns a string wrapped in an ANSI foreground color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="hexColor">
    ///     The color to use on the specified string.
    ///     <para>Supported format: [#]RRGGBB.</para>
    /// </param>
    public static string Pastel(this string input, string hexColor)
    {
        return HexColorFormatFuncs[_enabled][ColorPlane.Foreground](input, hexColor);
    }


    /// <summary>
    ///     Returns a string wrapped in an ANSI background color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string PastelBg(this string input, Color color)
    {
        return ColorFormatFuncs[_enabled][ColorPlane.Background](input, color);
    }

    /// <summary>
    ///     Returns a string wrapped in an ANSI background color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="hexColor">
    ///     The color to use on the specified string.
    ///     <para>Supported format: [#]RRGGBB.</para>
    /// </param>
    public static string PastelBg(this string input, string hexColor)
    {
        return HexColorFormatFuncs[_enabled][ColorPlane.Background](input, hexColor);
    }


    private static string CloseNestedPastelStrings(string input, Color color, ColorPlane colorPlane)
    {
        var closedString = CloseNestedPastelStringRegex1.Replace(input, FormatStringEnd);

        closedString = CloseNestedPastelStringRegex2.Replace(closedString, $"{FormatStringEnd}$0");
        closedString = CloseNestedPastelStringRegex3[colorPlane].Replace(closedString,
            $"$0{string.Format($"{FormatStringStart}{FormatStringColor}", PlaneFormatModifiers[colorPlane], color.R, color.G, color.B)}");

        return closedString;
    }
}