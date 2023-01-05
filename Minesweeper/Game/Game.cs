using Minesweeper.ConsoleDisplay;
using Minesweeper.UI;
using Timer = Minesweeper.UI.Timer;

namespace Minesweeper.Game;

public static class Game
{
    private static Grid _grid = null!;
    private static bool _gameIsRunning;
    
    private static readonly TimeSpan Time = TimeSpan.FromSeconds(200);

    private static DifficultyPreset _preset;
    private static int _remainingFlags;
    
    private static readonly EntryText FlagsText = new("", Color.Red);
    private static readonly TimerText TimerText = new(new Timer(Time), Color.Red);

    public static void Start(DifficultyPreset preset)
    {
        _preset = preset;
        _remainingFlags = preset.BombAmount;
        
        _grid = new Grid(preset);

        _grid.BombClicked += OnBombClicked;
        _grid.Flagged += ChangeFlagCount;

        DisplayInterface();

        _gameIsRunning = true;
    }

    private static void DisplayInterface()
    {
        // var frame = new Frame(3, 3)
        // {
        //     Pos = (1, 1)
        // };
        //
        // new Button(frame)
        // {
        //     Text =  new UString("Menu", Color.Black),
        //     DefaultColor = Color.DarkGray,
        //     HighlightedColor = Color.Gray,
        //     PressedColor = Color.White,
        //     
        //     Fill = FillMode.Both,
        //
        //     OnClick = () => 
        //     {
        //         frame.Clear();
        //         MainMenu();
        //     }
        // }.Grid(0, 0);
        //
        // FlagsText.Text = _preset.BombAmount.ToString();
        // new Label(frame)
        // {
        //     Text = FlagsText,
        //     DefaultColor = Color.DarkGray,
        //     Fill = FillMode.Horizontal
        // }.Grid(1, 0);
        //
        // new Button(frame)
        // {
        //     Text = new UString(":)", Color.Wheat),
        //     DefaultColor = Color.DarkGreen,
        //     HighlightedColor = Color.DarkGreen.Dimmer(),
        //     PressedColor = Color.Green,
        //
        //     OnClick = Restart
        // }.Grid(1, 1);
        //
        // TimerText.RestartTimer();
        // new Label(frame)
        // {
        //     Text = TimerText,
        //     DefaultColor = Color.DarkGray
        // }.Grid(1, 2);
        //
        // new Background(frame)
        // {
        //     DefaultColor = Color.Gray,
        // }.Grid(1, 0, columnSpan: 3);
        //
        // new Canvas(frame, _grid).Grid(2, 0, columnSpan: 3);
    }

    // private static void InputOnMouseClick(MouseState state)
    // {
    //     if (!_gameIsRunning) return;
    //     
    //     _grid.ClickTile(state.Position - _grid.Offset, state.Buttons);
    // }

    private static void OnBombClicked()
    {
        _gameIsRunning = false;
        TimerText.Animating = false;
    }

    private static void ChangeFlagCount(int change)
    {
        _remainingFlags += change;
        FlagsText.Text = _remainingFlags.ToString();

        FlagsText.Background = _remainingFlags < 0 ? Color.DarkRed : null;
    }

    private static void Restart()
    {
        TimerText.Animating = true;
        TimerText.RestartTimer();

        _remainingFlags = _preset.BombAmount;
        FlagsText.Text = _remainingFlags.ToString();
        FlagsText.Background = null;
        
        _grid.GenerateNew();
        _gameIsRunning = true;
    }

    private static void MainMenu()
    {
        _grid.BombClicked -= OnBombClicked;
        _grid.Flagged -= ChangeFlagCount;
        
        UI.MainMenu.Show();
    }
}

public record struct DifficultyPreset(string Name, int BombAmount, int GridWidth, int GridHeight);
