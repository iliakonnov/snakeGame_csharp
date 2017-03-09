using System;
using System.Drawing;
using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class SnakeConfig
    {
        NumericUpDown _speed;
        NumericUpDown _circleSize;
        NumericUpDown _circleOffset;
        NumericUpDown _initLen;
        ColorPicker _damageColor;
        CheckBox _headColorEnabled;
        ColorPicker _headColor;
        CheckBox _rainbowSnake;
        ColorPicker _snakeColor;

        MainGame.Config.SnakeConfigClass _config;
        public SnakeConfig(MainGame.Config.SnakeConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.SnakeConfigClass GetConfig()
        {
            var result = new MainGame.Config.SnakeConfigClass
            {
                Speed = (int)_speed.Value,
                CircleSize = (int)_circleSize.Value,
                CircleOffset = (int)_circleOffset.Value,
                InitLen = (int)_initLen.Value,
                DamageColor = ColorConverter.ToXna(_damageColor.Value),
                Colors = (_rainbowSnake.Checked ?? false)
                    ? null
                    : new []{ColorConverter.ToXna(_snakeColor.Value)}
            };
            if (_headColorEnabled.Checked ?? false)
            {
                result.HeadColor = ColorConverter.ToXna(_headColor.Value);
            }
            else
            {
                result.HeadColor = null;
            }
            return result;
        }

        public TabPage GetPage()
        {
            _speed = new NumericUpDown {Value = _config.Speed};
            _circleSize = new NumericUpDown {Value = _config.CircleSize};
            _circleOffset = new NumericUpDown {Value = _config.CircleOffset};
            _initLen = new NumericUpDown {Value = _config.InitLen};
            _damageColor = new ColorPicker {Value = ColorConverter.ToEto(_config.DamageColor)};
            _headColorEnabled = new CheckBox {Checked = _config.HeadColor != null};
            _headColor = new ColorPicker
            {
                Value = _config.HeadColor == null
                    ? ColorConverter.ToEto(Color.Transparent)
                    : ColorConverter.ToEto(_config.HeadColor.Value)
            };
            _rainbowSnake = new CheckBox {Checked = _config.Colors == null};
            _snakeColor = new ColorPicker
            {
                Value = _config.Colors == null
                    ? ColorConverter.ToEto(Color.Transparent)
                    : _config.Colors.Length != 1
                        ? ColorConverter.ToEto(Color.Transparent)
                        : ColorConverter.ToEto(_config.Colors[0])
            };

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _speed, new Label { Text = "Speed (px/s)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _circleSize, new Label { Text = "Circles radius (px)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _circleOffset, new Label { Text = "Offset between circles (px)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _initLen, new Label { Text = "Snake length"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _damageColor, new Label { Text = "Color when damage taken"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _headColorEnabled, new Label { Text = "Use special head color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _headColor, new Label { Text = "Special head color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _rainbowSnake, new Label { Text = "Rainbow snake"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _snakeColor, new Label { Text = "Snake color"} }
                    }
                    // TODO: Сделать настройку цветов змеи (Color[] Colors)
                }
            })
            {
                Text = "Snake"
            };
        }
    }
}