using System.Runtime.InteropServices;
using Minesweeper;

[DllImport("kernel32.dll")]
static extern IntPtr GetStdHandle(int nStdHandle);

[DllImport("kernel32.dll")]
static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

var displayMode = 0;

if (args.Length > 0)
{
    var arg = args[0];
    
    if (arg[0] is '-' or '/')
    {
        var param = arg[1..].ToLower();

        switch (param)
        {
            case "native" or "0":
                displayMode = 0;
                break;
            
            case "ansi" or "1":
                displayMode = 1;
                break;
            
            default:
                Console.WriteLine($"Unknown flag: {param}");
                return;
        }
    }
}
else
{
    var handle = GetStdHandle(-11);
    var mode = 0u;

    GetConsoleMode(handle, ref mode);
    
    displayMode = (mode & 0x4) != 0 ? 1 : 0;
}

Display.Init(displayMode);

Input.Init();

MainMenu.Display();
