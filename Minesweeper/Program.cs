using System.Drawing;
using Minesweeper;
using Minesweeper.UI;

// var text = "Enter".Pastel(Color.Blue);
// var text2 = "Oro".Color(Color.PaleVioletRed, Color.Cornsilk);
//
// Console.WriteLine(text);
// Console.WriteLine(text2);

var display = new AnsiDisplay();

display.DrawRect(new Coord(0, 0), new Coord(100, 300), Color.Aqua, Color.Brown);

// #pragma warning disable CA1416
// NativeDisplay.Init(50, 20);
// #pragma warning restore CA1416
//
// Input.Init();
//
// MainMenu.Display();
