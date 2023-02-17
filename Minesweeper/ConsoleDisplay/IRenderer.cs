﻿using Minesweeper.UI;
using Minesweeper.UI.Widgets;
using Minesweeper.Visual;

namespace Minesweeper.ConsoleDisplay;

public interface IRenderer
{
    void Draw(int posX, int posY, char symbol, Color fg, Color bg);

    void DrawRect(Vector start, Vector end, Color color, char symbol);

    void DrawLine(Vector pos, Vector direction, int length, Color fg, Color bg, char symbol);

    void Print(int posX, int posY, string text, Color fg, Color bg, Alignment alignment, TextMode mode);

    void DrawBuffer(Vector start, Vector end, Pixel[,] buffer);

    void DrawBorder(Vector start, Vector end, Color color, BorderStyle style);

    void ClearAt(int posX, int posY);

    void ClearRect(Vector start, Vector end);

    void Draw();

    void ResetStyle();

    void Clear();

    void ResizeBuffer(Vector newBufferSize);
}
