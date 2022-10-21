using Spectre.Console;

namespace Minesweeper;

public static class MainMenu
{
    private static bool _colorSupport = true;
    
    public static void Display()
    {
        if (AnsiConsole.Profile.Capabilities.ColorSystem == ColorSystem.NoColors)
        {
            _colorSupport = false;
        }
    }
}
    