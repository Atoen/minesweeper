using Minesweeper;
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
    
    var bytesPerMByte = Math.Pow(2, 20);
    Console.WriteLine(
        $"Memory usage - Physical: {peakPhysical / bytesPerMByte:.00}MB, Paged: {peakPaged / bytesPerMByte:.00}MB");
    
    Environment.Exit(Environment.ExitCode);
};

var displayMode = DisplayMode.Auto;

void ParseArgs(string arg)
{
    if (arg[0] is not ('/' or '-')) return;
    
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
            
            Environment.Exit(1);
            return;
    }
}

if (args.Length > 0) ParseArgs(args[0]);

Display.Init(displayMode);

Input.Init();

MainMenu.Show();
