using Spectre.Console;
using Spectre.Console.Rendering;

namespace Minesweeper;

public static class MainMenu
{
    public static void Display()
    {

        // No title
        Render(
            new Rule()
                .RuleStyle(Style.Parse("yellow"))
                .AsciiBorder()
                .LeftAligned());

        // Left aligned title
        Render(
            new Rule("[blue]Left aligned[/]")
                .RuleStyle(Style.Parse("red"))
                .DoubleBorder()
                .LeftAligned());

        // Centered title
        Render(
            new Rule("[green]Centered[/]")
                .RuleStyle(Style.Parse("green"))
                .HeavyBorder()
                .Centered());

        // Right aligned title
        Render(
            new Rule("[red]Right aligned[/]")
                .RuleStyle(Style.Parse("blue"))
                .RightAligned());
    }

    private static void Render(Rule rule)
    {
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }
}