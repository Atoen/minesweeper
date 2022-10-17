﻿using System.CodeDom.Compiler;

namespace Minesweeper;

public static class Grid
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    private static bool[] BombArray = Array.Empty<bool>();
    
    public static void Generate(int width, int height)
    {
        var random = new Random();
        BombArray = new bool[width * height];
        
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            if (random.NextDouble() > 0.9)
            {
                
                BombArray[x * height + y] = true;
                Display.Print(x, y, '*', ConsoleColor.Red, ConsoleColor.DarkRed);
                continue;
            }
            
            Display.Print(x, y, '#', ConsoleColor.Green, ConsoleColor.Black);
        }
    }
}