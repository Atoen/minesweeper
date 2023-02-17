using System.Text;

namespace Minesweeper.Visual.FigletText;

public class Figlet
{
    public string Text { get; }
    
    public FigletFont Font { get; }
    
    public CharacterWidth CharacterWidth { get; }
    
    public string[] Result { get; }

    public int Height => Font.Height;

    public int Width => Result.Max(line => line.Length);

    private readonly StringBuilder _builder = new();

    public Figlet(string text, FigletFont? font = null, CharacterWidth characterWidth = CharacterWidth.Fitted)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Contains(Environment.NewLine))
        {
            throw new ArgumentException("Text cannot contain newline character");
        }

        Text = text;
        CharacterWidth = characterWidth;
        Font = font ?? FigletFont.Default;
        Result = new string[Font.Height];

        if (Text.Length == 0) return;

        switch (CharacterWidth)
        {
            case CharacterWidth.Smush:
                GenerateSmush();
                break;
            
            case CharacterWidth.Fitted:
                GenerateFitted();
                break;
            
            case CharacterWidth.Full:
                GenerateFull();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(characterWidth));
        }
    }

    private void GenerateFitted()
    {
        for (var line = 0; line < Height; line++)
        {
            foreach (var symbol in Text)
            {
                _builder.Append(Font.GetCharacterLine(symbol, line));
            }

            Result[line] = _builder.ToString();
            _builder.Clear();
        }
    }

    private void GenerateSmush()
    {
        for (var line = 0; line < Height; line++)
        {
            _builder.Append(Font.GetCharacterLine(Text[0], line));
            var lastChar = Text[0];

            for (var charIndex = 1; charIndex < Text.Length; charIndex++)
            {
                var currentChar = Text[charIndex];
                var currentCharacterLine = Font.GetCharacterLine(currentChar, line);
                
                if (lastChar != ' ' && currentChar != ' ')
                {
                    if (_builder[^1] == ' ')
                    {
                        _builder[^1] = currentCharacterLine[0];
                    }

                    _builder.Append(currentCharacterLine[1..]);
                }
                else
                {
                    _builder.Append(currentCharacterLine);
                }

                lastChar = currentChar;
            }
            
            Result[line] = _builder.ToString();
            _builder.Clear();
        }
    }

    private void GenerateFull()
    {
        for (var line = 0; line < Height; line++)
        {
            foreach (var symbol in Text)
            {
                _builder.Append(Font.GetCharacterLine(symbol, line));
                _builder.Append(' ');
            }
            
            Result[line] = _builder.ToString();
            _builder.Clear();
        }
    }

    public override string ToString()
    {
        foreach (var line in Result)
        {
            _builder.AppendLine(line);
        }

        return _builder.ToString().TrimEnd();
    }
}

public enum CharacterWidth
{
    Smush,
    Fitted,
    Full
}