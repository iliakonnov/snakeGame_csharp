﻿using System;
using Eto.Drawing;
using Eto.Forms;
using Color = Microsoft.Xna.Framework.Color;

namespace snake_game.Launcher.Config
{
    public class GameConfig
    {
        MainGame.Config.GameConfigClass _config;
        NumericUpDown _lives;
        NumericUpDown _foodToLive;
        ColorPicker _textColor;
        ColorPicker _bgColor;
        ColorPicker _fogColor;
        CheckBox _fogEnabled;
        NumericUpDown _fogSize;


        public GameConfig(MainGame.Config.GameConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.GameConfigClass GetConfig()
        {
            return new MainGame.Config.GameConfigClass
            {
                Lives = (int)_lives.Value,
                ScoreToLive = (int)_foodToLive.Value,
                TextColor = Utils.ColorConverter.ToXna(_textColor.Value),
                FogEnabled = _fogEnabled.Checked ?? false,
                FogColor = new Tuple<Color, Color>(Utils.ColorConverter.ToXna(_fogColor.Value), Color.Transparent),
                FogSize = _fogSize.Value,
                BackgroundColor = Utils.ColorConverter.ToXna(_bgColor.Value),
            };
        }

        public TabPage GetPage()
        {
            _lives = new NumericUpDown { Value = _config.Lives };
            _foodToLive = new NumericUpDown { Value = _config.ScoreToLive };
            _textColor = new ColorPicker { Value = Utils.ColorConverter.ToEto(_config.TextColor) };
            _bgColor = new ColorPicker { Value = Utils.ColorConverter.ToEto(_config.BackgroundColor) };
            _fogColor = new ColorPicker { Value = Utils.ColorConverter.ToEto(_config.FogColor.Item1) };
            _fogEnabled = new CheckBox { Checked = _config.FogEnabled };
            _fogSize = new NumericUpDown { Value = _config.FogSize };


            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {_lives, new Label {Text = "Lives", VerticalAlignment = VerticalAlignment.Center}}
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _foodToLive,
                            new Label {Text = "Food to get extra live", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _textColor,
                            new Label {Text = "Information text color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _bgColor,
                            new Label {Text = "Background color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogColor,
                            new Label {Text = "Fog color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new Label {Height = 2}, // Separator
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogEnabled,
                            new Label {Text = "Enable fog", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogSize,
                            new Label {Text = "Fog size", VerticalAlignment = VerticalAlignment.Center}
                        }
                    }
                }
            })
            {
                Text = "Game"
            };
        }
    }
}