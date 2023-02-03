﻿using Minesweeper.ConsoleDisplay;
using Minesweeper.UI.Events;

namespace Minesweeper.UI.Widgets;

public class Entry : ContentControl
{
    public Entry() => _text = new EntryText(nameof(Entry)) {Parent = this};

    private EntryText _text;
    public EntryText Text
    {
        get => _text;
        set
        {
            _text.Remove();
            _text.Parent = null!;
            
            _text = value;
            _text.Parent = this;

            MaxTextLenght = _text.Length;
        }
    }

    public EntryMode InputMode { get; set; } = EntryMode.All;
    public TextMode TextModeWhileTyping { get; set; } = TextMode.Italic;
    
    public int MaxTextLenght { get; set; }

    private bool _inEntryMode;
    
    private bool CheckIfAllowed(char symbol)
    {
        return InputMode switch
        {
            EntryMode.Alphanumeric => char.IsLetterOrDigit(symbol),
            EntryMode.Letters => char.IsLetter(symbol),
            EntryMode.Digits => char.IsDigit(symbol),
            _ => !char.IsControl(symbol)
        };
    }

    public override void Remove()
    {
        _text.Remove();
        base.Remove();
    }

    public override void Clear()
    {
        base.Clear();
        _text.Clear();
    }

    private void EnterEntryMode()
    {
        _inEntryMode = true;
        Text.TextMode = TextModeWhileTyping;
        Text.Animating = true;
    }

    private void ExitEntryMode()
    {
        _inEntryMode = false;
        Text.Animating = false;
        Text.TextMode = TextMode.Default;
        
        if (InputMode != EntryMode.Digits) return;

        if (string.IsNullOrWhiteSpace(Text.String)) Text.String = "0";
    }

    private void EnterText(KeyboardEventArgs e)
    {
        if (e.IsReleased) return;

        if (e.Key == ConsoleKey.Enter)
        {
            ExitEntryMode();
            return;
        }

        if (CheckIfAllowed(e.Char) && Text.Length < MaxTextLenght)
        {
            if (Text.String == "0" && InputMode == EntryMode.Digits)
            {
                Text.String = e.Char.ToString();
            }
            else Text.Append(e.Char);
            
            return;
        }

        if (e.Key == ConsoleKey.Backspace && Text.Length > 0)
        {
            Text.RemoveLast();
        }
    }
    
    protected override void OnKeyDown(KeyboardEventArgs e)
    {
        if (IsFocused && !_inEntryMode && e.IsPressed) EnterEntryMode();
        
        if (_inEntryMode) EnterText(e);
            
        base.OnKeyDown(e);
    }

    protected override void OnMouseLeftDown(MouseEventArgs e)
    {
        if (_inEntryMode) ExitEntryMode();
        else EnterEntryMode();

        base.OnMouseLeftDown(e);
    }

    protected override void OnLostFocus(InputEventArgs e)
    {
        ExitEntryMode();
        
        base.OnLostFocus(e);
    }

    public override void Resize()
    {
        MinSize = InnerPadding * 2 + (MaxTextLenght + 1, 1);

        Size = ResizeMode switch
        {
            ResizeMode.Grow => Size.ExpandTo(MinSize),
            ResizeMode.Stretch => MinSize,
            _ => Size
        };

        base.Resize();
    }
}

public enum EntryMode
{
    All,
    Alphanumeric,
    Letters,
    Digits,
}
