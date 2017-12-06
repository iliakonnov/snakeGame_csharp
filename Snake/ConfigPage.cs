using Eto.Forms;
using Microsoft.Xna.Framework;
using snake_game.Bonuses;
using snake_game.Utils;

namespace Snake
{
    public class ConfigPage : IConfigPage
    {
        CheckBox _enabled;
        NumericUpDown _speed;
        NumericUpDown _circleSize;
        NumericUpDown _damageTimeout;
        NumericUpDown _circleOffset;
        NumericUpDown _initLen;
        ColorPicker _damageColor;
        CheckBox _headColorEnabled;
        ColorPicker _headColor;
        CheckBox _rainbowSnake;
        ColorPicker _snakeColor;
        ComboBox _controlType;
        NumericUpDown _turnSize;

        Config _config;

        public ConfigPage(Config config)
        {
            _config = config;
        }

        public IPluginConfig GetConfig()
        {
            var result = new Config
            {
                Speed = (int) _speed.Value,
                DamageTimeout = (int) (_damageTimeout.Value * 1000),
                CircleSize = (int) _circleSize.Value,
                CircleOffset = (int) _circleOffset.Value,
                InitLen = (int) _initLen.Value,
                DamageColor = ColorConverter.ToXna(_damageColor.Value),
                Colors = (_rainbowSnake.Checked ?? false)
                    ? null
                    : new[] {ColorConverter.ToXna(_snakeColor.Value)},
                IsEnabled = _enabled.Checked ?? false,
                ControlType = _controlType.SelectedKey,
                TurnSize = (int) _turnSize.Value
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
            _turnSize = _config.TurnSize != null
                ? new NumericUpDown {Value = (double) _config.TurnSize}
                : new NumericUpDown {Value = 0};
            _speed = new NumericUpDown {Value = _config.Speed};
            _circleSize = new NumericUpDown {Value = _config.CircleSize};
            _damageTimeout = new NumericUpDown {Value = _config.DamageTimeout / 1000};
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
            _enabled = new CheckBox {Checked = _config.IsEnabled};
            _controlType = new ComboBox();
            _controlType.Items.Add("traditional");
            _controlType.Items.Add("small");
            _controlType.SelectedKey = _config.ControlType;

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
                            _speed,
                            new Label {Text = "Speed (px/s)", VerticalAlignment = VerticalAlignment.Center}
                        }
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
                            _circleSize,
                            new Label {Text = "Circles radius (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _circleOffset,
                            new Label
                            {
                                Text = "Offset between circles (px)",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _initLen,
                            new Label {Text = "Snake length", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _damageColor,
                            new Label {Text = "Color when damage taken", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _headColorEnabled,
                            new Label {Text = "Use special head color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _headColor,
                            new Label {Text = "Special head color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _rainbowSnake,
                            new Label {Text = "Rainbow snake", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _snakeColor,
                            new Label {Text = "Snake color", VerticalAlignment = VerticalAlignment.Center}
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
                            new Label
                            {
                                Text = "Turn size (for some control types)",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
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