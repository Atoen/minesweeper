﻿using System.Collections;
using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Widgets;

namespace Minesweeper.UI;

public class EntryText : Text
{
    public EntryText(string text) : this(text, Color.Black)
    {
    }

    public EntryText(string text, Color foreground) : this(text, foreground, Color.Transparent)
    {
    }

    public EntryText(string text, Color foreground, Color background) : base(text, foreground, background)
    {
        _enumerator = Cycle();
        Size = new Vector(Length + 1, 1);
    }

    public char Caret { get; set; } = '_';

    public bool Animating
    {
        get => _animating;
        set
        {
            if (!value) _displayingCaret = false;
            _animating = value;
        }
    }

    public int CycleSpeed { get; set; } = 10;

    private bool _displayingCaret;
    private bool _animating;
    private readonly IEnumerator _enumerator;

    public override string String
    {
        get => TextInternal;
        set
        {
            TextInternal = value;
            Size = new Vector(Length + 1, 1);
        }
    }

    public void Append(string text) => TextInternal += text;

    public void Append(char symbol) => TextInternal += symbol;

    public void RemoveLast(int n = 1) => TextInternal = TextInternal[..^n];
    
    private IEnumerator Cycle()
    {
        var i = 0;

        while (Enabled)
        {
            i++;

            if (i >= CycleSpeed)
            {
                _displayingCaret = !_displayingCaret;
                i = 0;
            }

            yield return null;
        }
    }

    public override void Render()
    {
        if (Parent == null) return;
        
        if (Animating) _enumerator.MoveNext();

        var background = Background == Color.Transparent ? Parent.CurrentColor : Background;
        var position = Parent.Center;

        var posX = position.X;
        if (Length % 2 != 0 && Animating) posX--;

        Display.Print(posX, position.Y, TextInternal, Foreground, background, Alignment, TextMode);

        if (!_displayingCaret) return;
        
        var startX = Alignment switch
        {
            Alignment.Left => position.X,
            Alignment.Right => position.X - Length + 1,
            _ => position.X - (Length + 1) / 2
        };
            
        Display.Draw(startX + TextInternal.Length, position.Y, Caret, Foreground, background);
    }
}