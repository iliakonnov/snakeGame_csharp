using Eto.Forms;

namespace snake_game.Launcher.Bonuses
{
    public class AppleConfig : IBonusConfig<MainGame.Config.BonusConfigClass.AppleConfigClass>
    {
        CheckBox _enabled;
        NumericUpDown _appleCount;
        NumericUpDown _thickness;
        NumericUpDown _radius;
        NumericUpDown _sides;
        NumericUpDown _speed;
        NumericUpDown _bounceTimeout;
        ColorPicker _appleColor;
        MainGame.Config.BonusConfigClass.AppleConfigClass _config;

        public AppleConfig(MainGame.Config.BonusConfigClass.AppleConfigClass config, bool enabled)
        {
            _enabled = new CheckBox {Checked = enabled};
            _config = config;
        }

        public MainGame.Config.BonusConfigClass.AppleConfigClass GetConfig()
        {
            return new MainGame.Config.BonusConfigClass.AppleConfigClass
            {
                AppleCount = (int) _appleCount.Value,
                Thickness = (float) _thickness.Value,
                Radius = (int) _radius.Value,
                Sides = (int) _sides.Value,
                Speed = (int) _speed.Value,
                AppleColor = ColorConverter.ToXna(_appleColor.Value),
                BounceTimeout = (float) _bounceTimeout.Value * 1000
            };
        }

        public bool IsEnabled()
        {
            return _enabled.Checked ?? false;
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