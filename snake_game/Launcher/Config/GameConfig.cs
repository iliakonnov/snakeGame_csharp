using System;
using Eto.Drawing;
using Eto.Forms;
using Color = Microsoft.Xna.Framework.Color;

namespace snake_game.Launcher.Config
{
    public class GameConfig
    {
        MainGame.Config.GameConfigClass _config;
        NumericUpDown _lives;
        NumericUpDown _damageTimeout;
        NumericUpDown _foodToLive;
        ColorPicker _textColor;
        ColorPicker _bgColor;
        ColorPicker _fogColor;
        CheckBox _debugEnabled;
        ColorPicker _debugColor;
        CheckBox _fogEnabled;
        NumericUpDown _fogSizeMultiplier;
        ComboBox _controlType;
        NumericUpDown _turnSize;


        public GameConfig(MainGame.Config.GameConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.GameConfigClass GetConfig()
        {
            return new MainGame.Config.GameConfigClass
            {
                Lives = (int)_lives.Value,
                DamageTimeout = (int)(_damageTimeout.Value * 1000),
                FoodToLive = (int)_foodToLive.Value,
                TextColor = ColorConverter.ToXna(_textColor.Value),
                DebugShow = _debugEnabled.Checked ?? false,
                DebugColor = ColorConverter.ToXna(_debugColor.Value),
                FogEnabled = _fogEnabled.Checked ?? false,
                FogColor = new Tuple<Color, Color>(ColorConverter.ToXna(_fogColor.Value), Color.Transparent),
                FogSizeMultiplier = _fogSizeMultiplier.Value,
                BackgroundColor = ColorConverter.ToXna(_bgColor.Value),
                ControlType = _controlType.SelectedValue.ToString(),
                TurnSize = _turnSize.Value > 0? (int?)_turnSize.Value : null
            };
        }

        public TabPage GetPage()
        {
            _lives = new NumericUpDown { Value = _config.Lives };
            _damageTimeout = new NumericUpDown { Value = _config.DamageTimeout / 1000 };
            _foodToLive = new NumericUpDown { Value = _config.FoodToLive };
            _textColor = new ColorPicker { Value = ColorConverter.ToEto(_config.TextColor) };
            _bgColor = new ColorPicker { Value = ColorConverter.ToEto(_config.BackgroundColor) };
            _fogColor = new ColorPicker { Value = ColorConverter.ToEto(_config.FogColor.Item1) };
            _debugEnabled = new CheckBox { Checked = _config.DebugShow };
            _debugColor = new ColorPicker { Value = ColorConverter.ToEto(_config.DebugColor) };
            _fogEnabled = new CheckBox { Checked = _config.FogEnabled };
            _fogSizeMultiplier = new NumericUpDown { Value = _config.FogSizeMultiplier };
            _controlType = new ComboBox();
            _controlType.Items.Add("traditional");
            _controlType.Items.Add("small");
            _controlType.Text = _config.ControlType;
            _turnSize = new NumericUpDown { Value = _config.TurnSize.HasValue ? (int)_config.TurnSize : 0 };

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
                            _damageTimeout,
                            new Label {Text = "Damage timeout (sec)", VerticalAlignment = VerticalAlignment.Center}
                        }
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
                            _debugEnabled,
                            new Label {Text = "Enable debug (~)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _debugColor,
                            new Label {Text = "Debug window color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
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
                            _fogSizeMultiplier,
                            new Label {Text = "Fog size multiplier", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _controlType,
                            new Label {Text = "Type of control", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _turnSize,
                            new Label {Text = "Turn size (for some control types)", VerticalAlignment = VerticalAlignment.Center}
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