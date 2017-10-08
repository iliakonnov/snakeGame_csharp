using Eto.Forms;
using snake_game.Bonuses;
using snake_game.Launcher;

namespace AntiAppleBonus
{
    public class ConfigPage: IConfigPage
    {
        CheckBox _enabled;
        NumericUpDown _startSnakeLength;
        NumericUpDown _chanceTime;
        NumericUpDown _size;
        NumericUpDown _newChance;
        NumericUpDown _thickness;
        ColorPicker _color;
        Config _config;

        public ConfigPage(Config config)
        {
            _config = config;
        }

        public IPluginConfig GetConfig()
        {
            return new Config
            {
                IsEnabled = _enabled.Checked ?? false,
                StartSnakeLength = (int) _startSnakeLength.Value,
                ChanceTime = (int) _chanceTime.Value * 1000,
                Size = (int) _size.Value,
                NewChance = (int) _newChance.Value / 100.0,
                Thickness = (float) _thickness.Value,
                Color = ColorConverter.ToXna(_color.Value)
            };
        }

        public TabPage GetPage()
        {
            _startSnakeLength = new NumericUpDown {Value = _config.StartSnakeLength};
            _chanceTime = new NumericUpDown {Value = _config.ChanceTime / 1000.0};
            _size = new NumericUpDown {Value = _config.Size};
            _newChance = new NumericUpDown {Value = _config.NewChance * 100};
            _thickness = new NumericUpDown {Value = _config.Thickness};
            _color = new ColorPicker {Value = ColorConverter.ToEto(_config.Color)};
            _enabled = new CheckBox {Checked = _config.IsEnabled};
            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _enabled,
                            new Label {Text = "Bonus enabled", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _startSnakeLength,
                            new Label {Text = "Minimum snake length", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _thickness,
                            new Label {Text = "Triangle thickness (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _size,
                            new Label {Text = "Size (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _color,
                            new Label {Text = "Color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _chanceTime,
                            new Label {Text = "Chance time (sec)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _newChance,
                            new Label
                            {
                                Text = "Chance for bonus spawn (%)",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    }
                }
            })
            {
                Text = "AntiApple"
            };
        }
    }
}