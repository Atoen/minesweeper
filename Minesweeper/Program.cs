using System.Text;
using Minesweeper;
using Minesweeper.UI;

var display = new AnsiDisplay();

// display.Draw(5, 3, '4', ConsoleColor.Black, ConsoleColor.Black);

// var stdout = Console.OpenStandardOutput();
// var con = new StreamWriter(stdout, Encoding.ASCII);
// con.AutoFlush = true;
// Console.SetOut(con);

Console.WriteLine("\x1b[36mTEST\x1b[0m");

for (var i = 0; i < 255; i++)
{
    display.Draw2(i, 2, i.ToString(), (ConsoleColor) i, ConsoleColor.Black);
}

// #pragma warning disable CA1416
// NativeDisplay.Init(50, 20);
// #pragma warning restore CA1416
//
// Input.Init();
//
// MainMenu.Display();
