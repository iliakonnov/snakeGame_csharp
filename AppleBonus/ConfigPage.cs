using Eto.Forms;
using snake_game.Bonuses;
using snake_game.Launcher;

namespace snake_plugins.AppleBonus
{
    public class ConfigPage : IConfigPage
    {
        CheckBox _enabled;
        NumericUpDown _appleCount;
        NumericUpDown _thickness;
        NumericUpDown _radius;
        NumericUpDown _sides;
        NumericUpDown _speed;
        NumericUpDown _bounceTimeout;
        ColorPicker _appleColor;
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
                AppleCount = (int) _appleCount.Value,
                Thickness = (float) _thickness.Value,
                Radius = (int) _radius.Value,
                Sides = (int) _sides.Value,
                Speed = (int) _speed.Value,
                AppleColor = ColorConverter.ToXna(_appleColor.Value),
                BounceTimeout = (float) _bounceTimeout.Value * 1000
            };
        }

        public TabPage GetPage()
        {
            _appleCount = new NumericUpDown {Value = _config.AppleCount};
            _thickness = new NumericUpDown {Value = _config.Thickness};
            _radius = new NumericUpDown {Value = _config.Radius};
            _sides = new NumericUpDown {Value = _config.Sides};
            _speed = new NumericUpDown {Value = _config.Speed};
            _bounceTimeout = new NumericUpDown {Value = _config.BounceTimeout / 1000};
            _appleColor = new ColorPicker {Value = ColorConverter.ToEto(_config.AppleColor)};
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
                            _appleCount,
                            new Label {Text = "Apple count", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _speed,
                            new Label {Text = "Apple speed (px/s)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _radius,
                            new Label {Text = "Apple radius (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _appleColor,
                            new Label {Text = "Apple color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _thickness,
                            new Label {Text = "Circle thickness (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _bounceTimeout,
                            new Label {Text = "Bounce timeout (sec)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _sides,
                            new Label {Text = "Circle quality", VerticalAlignment = VerticalAlignment.Center}
                        }
                    }
                }
            })
            {
                Text = "Apple"
            };
        }
    }
}