﻿using Minesweeper;
using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;

Console.CancelKeyPress += delegate
{
    using var process = Process.GetCurrentProcess();

    process.Refresh();
    var peakPhysical = process.PeakWorkingSet64;
    var peakPaged = process.PeakPagedMemorySize64;

    Input.Stop();
    Display.Stop();

    Display.ResetStyle();

    Console.Clear();
    Console.WriteLine("Exiting...");

    const double bytesPerMByte = 1_048_576D;
    Console.WriteLine(
        $"Memory usage - Physical: {peakPhysical / bytesPerMByte:.00}MB, Paged: {peakPaged / bytesPerMByte:.00}MB");

    Environment.Exit(Environment.ExitCode);
};

var displayMode = DisplayMode.Auto;

void ParseArgs(string arg)
{
    var argSpan = arg.ToLower().AsSpan();

    if (argSpan[0] is not ('/' or '-')) return;

    var mode = argSpan[1..];

    switch (mode)
    {
        case "0" or "native":
            displayMode = DisplayMode.Native;
            break;

        case "1" or "ansi":
            displayMode = DisplayMode.Ansi;
            break;

        case "auto":
            break;

        default:
            Console.WriteLine($"Unknown display mode: {mode}");
            Console.WriteLine("Modes: native (0), ansi (1), auto");

            Environment.Exit(1);
            return;
    }
}

if (args.Length > 0) ParseArgs(args[0]);

Application.Start(displayMode);

MainMenu.Show();
