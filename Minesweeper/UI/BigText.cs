using Minesweeper.ConsoleDisplay;
using Minesweeper.Visual.FigletText;

namespace Minesweeper.UI;

public class BigText : Text
{
    private CharacterWidth _characterWidth;
    private FigletFont _font;

    public BigText(string text, FigletFont? font = null, CharacterWidth characterWidth = CharacterWidth.Fitted) : base(text, Color.Black)
    {
        _font = font ?? FigletFont.Default;
        _characterWidth = characterWidth;

        Figlet = new Figlet(text, Font, characterWidth);

        Size = new Vector(Figlet.Width, Figlet.Height);
    }

    public FigletFont Font
    {
        get => _font;
        set
        {
            if (value == _font) return;
            
            _font = value;
            GenerateNewText();
        }
    }

    public CharacterWidth CharacterWidth
    {
        get => _characterWidth;
        set
        {
            if (value == _characterWidth) return;
            
            _characterWidth = value;
            GenerateNewText();
        }
    }

    public Figlet Figlet { get; private set; }

    public override string String
    {
        get => TextInternal;
        set
        {
            if (value == TextInternal) return;
            
            TextInternal = value;
            GenerateNewText();
        }
    }
    
    private void GenerateNewText()
    {
        Figlet = new Figlet(TextInternal, _font, _characterWidth);
        Size = new Vector(Figlet.Width, Figlet.Height);
    }

    public override void Render()
    {
        if (Parent == null) return;
        
        var background = Background == Color.Transparent ? Parent.CurrentColor : Background;
        var position = Parent.Center;

        for (var i = 0; i < Height; i++)
        {
            var offset = i - Height / 2;
            
            Display.Print(position.X, position.Y + offset, Figlet.Result[i], Foreground, background, Alignment, TextMode);
        }
    }
}