using Minesweeper;
using Minesweeper.Display;
using Minesweeper.UI;

Console.CancelKeyPress += delegate
{
    using var process = Process.GetCurrentProcess();
    
    process.Refresh();
    var peakPhysical = process.PeakWorkingSet64;
    var peakPaged = process.PeakPagedMemorySize64;
    
    Input.Stop();
    Display.Stop();
    
    Console.Clear();
    Console.WriteLine("Exiting...");
    Console.WriteLine(
        $"Memory usage - Physical: {peakPhysical / (1024 * 1024)}MB, Paged: {peakPaged / (1024 * 1024)}MB");
    
    Environment.Exit(Environment.ExitCode);
};

var arg = args[0];
var displayMode = DisplayMode.Auto;

if (arg[0] is '/' or '-')
{
    var mode = arg[1..].ToLower();

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
            return;
    }
}

Display.Init(displayMode);

Input.Init();

MainMenu.Display();