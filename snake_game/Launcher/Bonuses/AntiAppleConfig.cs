using Eto.Forms;

namespace snake_game.Launcher.Bonuses
{
    public class AntiAppleConfig : IBonusConfig<MainGame.Config.BonusConfigClass.AntiAppleConfigClass>
    {
        CheckBox _enabled;
        NumericUpDown _startSnakeLength;
        NumericUpDown _chanceTime;
        NumericUpDown _size;
        NumericUpDown _newChance;
        NumericUpDown _thickness;
        ColorPicker _color;
        MainGame.Config.BonusConfigClass.AntiAppleConfigClass _config;

        public AntiAppleConfig(MainGame.Config.BonusConfigClass.AntiAppleConfigClass config, bool enabled)
        {
            _enabled = new CheckBox {Checked = enabled};
            _config = config;
        }

        public MainGame.Config.BonusConfigClass.AntiAppleConfigClass GetConfig()
        {
            return new MainGame.Config.BonusConfigClass.AntiAppleConfigClass
            {
                StartSnakeLength = 10,
                ChanceTime = (int) _chanceTime.Value * 1000,
                Size = (int) _size.Value,
                NewChance = (int) _newChance.Value / 100,
                Thickness = (float) _thickness.Value,
                Color = ColorConverter.ToXna(_color.Value)
            };
        }

        public bool IsEnabled()
        {
            return _enabled.Checked ?? false;
        }

        public TabPage GetPage()
        {
            _startSnakeLength = new NumericUpDown {Value = _config.StartSnakeLength};
            _chanceTime = new NumericUpDown {Value = _config.ChanceTime / 1000};
            _size = new NumericUpDown {Value = _config.Size};
            _newChance = new NumericUpDown {Value = _config.NewChance * 100};
            _thickness = new NumericUpDown {Value = _config.Thickness};
            _color = new ColorPicker {Value = ColorConverter.ToEto(_config.Color)};
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