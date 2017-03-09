using Eto.Forms;

namespace snake_game.Launcher.Bonuses
{
    public class AppleConfig
    {
        NumericUpDown _appleCount;
        NumericUpDown _thickness;
        NumericUpDown _radius;
        NumericUpDown _sides;
        NumericUpDown _speed;
        ColorPicker _appleColor;
        MainGame.Config.BonusConfigClass.AppleConfigClass _config;

        public AppleConfig(MainGame.Config.BonusConfigClass.AppleConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.BonusConfigClass.AppleConfigClass GetConfig()
        {
            return new MainGame.Config.BonusConfigClass.AppleConfigClass
            {
                AppleCount = (int) _appleCount.Value,
                Thickness = (int) _thickness.Value,
                Radius = (int) _radius.Value,
                Sides = (int) _sides.Value,
                Speed = (int) _speed.Value,
                AppleColor = ColorConverter.ToXna(_appleColor.Value)
            };
        }

        public TabPage GetPage()
        {
            _appleCount = new NumericUpDown {Value = _config.AppleCount};
            _thickness = new NumericUpDown {Value = _config.Thickness};
            _radius = new NumericUpDown {Value = _config.Radius};
            _sides = new NumericUpDown {Value = _config.Sides};
            _speed = new NumericUpDown {Value = _config.Speed};
            _appleColor = new ColorPicker {Value = ColorConverter.ToEto(_config.AppleColor)};
            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _appleCount, new Label { Text = "Apple count"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _speed, new Label { Text = "Apple speed"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _radius, new Label { Text = "Apple size"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _appleColor, new Label { Text = "Apple color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _thickness, new Label { Text = "Circle thickness"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _sides, new Label { Text = "Circle quality"} }
                    }
                }
            })
            {
                Text = "Apple"
            };
        }
    }

}