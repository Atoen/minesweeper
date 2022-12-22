﻿using Minesweeper.ConsoleDisplay;
using Minesweeper.Game;

namespace Minesweeper.UI;

public static class MainMenu
{
    private static readonly List<DifficultyPreset> Presets = new()
    {
        new DifficultyPreset("Easy", 10, 12, 8),
        new DifficultyPreset("Medium", 20, 24, 12),
        new DifficultyPreset("Hard", 50, 30, 15),
    };

    public static void Show2()
    {
        var frame = new Frame(2, 3)
        {
            Pos = (1, 1)
        };
        
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
        }.Grid(0, 0);
        
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
        }.Grid(1, 0);
        
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
        }.Grid(0, 1);
        
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
        }.Grid(1, 1, columnSpan: 2);
        
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,
        }.Grid(0, 2);
    }

    public static void Show()
    {
        var frame = new Frame(Presets.Count + 4, 4)
        {
            Pos = (1, 1)
        };

        var labelsRowOffset = 2;

        // Title label
        new Label(frame)
        {
            Text = new UString("MINESWEEPER", Color.DarkBlue)
            {
                Mode = TextMode.Bold
            },
            
            DefaultColor = Color.PaleGoldenrod,
        }.Grid(0, 1, columnSpan: 2);

        // Titles for preset values
        new Label(frame)
        {
            Text = new UString("Bombs", Color.Black),
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal
        }.Grid(1, 1);
        
        new Label(frame)
        {
            Text = new UString("Width", Color.Black),
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal
        }.Grid(1, 2);
        
        new Label(frame)
        {
            Text = new UString("Height", Color.Black),
            DefaultColor = Color.DarkGray,
            Fill = FillMode.Horizontal,
        }.Grid(1, 3);
        
        // Presets values
        var gradient = Colors.Gradient(Color.Green, Color.Orange, Presets.Count).ToList();
        var radioVariable = new Variable(1);
        
        for (var i = 0; i < Presets.Count; i++)
        {
            var preset = Presets[i];
        
            new RadioButton(frame, radioVariable, i)
            {
                Text = new UString(preset.Name, Color.Black),
                DefaultColor = gradient[i],
                HighlightedColor = gradient[i].Dimmer(),
                PressedColor = gradient[i].Brighter(),
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 0);
            
            new Label(frame)
            {
                Text = new UString(preset.BombAmount.ToString(), Color.Black),
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 1);
            
            new Label(frame)
            {
                Text = new UString(preset.GridWidth.ToString(), Color.Black),
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 2);
            
            new Label(frame)
            {
                Text =  new UString(preset.GridHeight.ToString(), Color.Black),
                DefaultColor = Color.DarkGray,
                Fill = FillMode.Horizontal
            }.Grid(i + labelsRowOffset, 3);
        }
        
        new RadioButton(frame, radioVariable, Presets.Count)
        {
            Text = new UString("Custom", Color.Black),
            DefaultColor = Color.CornflowerBlue,
            HighlightedColor = Color.CornflowerBlue.Dimmer(),
            PressedColor = Color.CornflowerBlue.Brighter()
        }.Grid(5, 0);
        
        var bombsEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "50",
            
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        
        }.Grid(5, 1);
        
        var widthEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "30",
        
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        }.Grid(5, 2);
        
        var heightEntry = new Entry(frame)
        {
            DefaultColor = Color.DarkGray,
            MaxTextLenght = 4,
            TextBackground = Color.Gray,
            Text = "20",
        
            Fill = FillMode.Horizontal,
            InputMode = TextEntryMode.Digits
        }.Grid(5, 3);

        // Play button
        new Button(frame)
        {
            Text = new UString("PLAY", Color.Black),
            DefaultColor = Color.White,
            HighlightedColor = Color.Yellow,
            PressedColor = Color.Lime,

            OnClick = () =>
            {
                frame.Clear();
                StartGame(GetSettings());
            }
        }.Grid(labelsRowOffset + Presets.Count + 1, 1, columnSpan: 2);
        
        // Background for difficulty presets
        new Background(frame)
        {
            DefaultColor = Color.Gray,
            InnerPadding = (1, 1)
        }.Grid(1, 0, rowSpan: Presets.Count + 2, columnSpan: 4);

        DifficultyPreset GetSettings()
        {
            var selected = radioVariable.Val;
            
            if (selected >= 0 && selected < Presets.Count)
                return Presets[selected];
            
            if (selected == Presets.Count)
            {
                return new DifficultyPreset
                {
                    BombAmount = int.Parse(bombsEntry.Text.Text),
                    GridWidth = int.Parse(widthEntry.Text.Text),
                    GridHeight = int.Parse(heightEntry.Text.Text)
                };
            }

            return new DifficultyPreset();
        }
    }

    private static void StartGame(DifficultyPreset preset)
    {
        Game.Game.Start(preset);
    }
}

