using Eto.Forms;
using snake_game.Bonuses;
using snake_game.Utils;

namespace BrickBonus
{
    /// <inheritdoc />
    public class ConfigPage : IConfigPage
    {
        private readonly Config _config;
        private ColorPicker _brickColor;
        private NumericUpDown _chanceTime;
        private CheckBox _enabled;
        private NumericUpDown _moveChance;
        private NumericUpDown _newChance;
        private NumericUpDown _size;
        private NumericUpDown _step;

        /// <inheritdoc />
        public ConfigPage(Config config)
        {
            _config = config;
        }

        /// <inheritdoc />
        public IPluginConfig GetConfig()
        {
            return new Config
            {
                IsEnabled = _enabled.Checked ?? false,
                ChanceTime = (int) _chanceTime.Value * 1000,
                MoveChance = _moveChance.Value / 100,
                NewChance = _newChance.Value / 100,
                Step = (int) _step.Value * 2,
                BrickColor = ColorConverter.ToXna(_brickColor.Value),
                Size = (int) _size.Value
            };
        }

        /// <inheritdoc />
        public TabPage GetPage()
        {
            _chanceTime = new NumericUpDown {Value = _config.ChanceTime / 1000.0};
            _moveChance = new NumericUpDown {Value = _config.MoveChance * 100};
            _newChance = new NumericUpDown {Value = _config.NewChance * 100};
            _step = new NumericUpDown {Value = _config.Step / 2.0};
            _size = new NumericUpDown {Value = _config.Size};
            _brickColor = new ColorPicker {Value = ColorConverter.ToEto(_config.BrickColor)};
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
                            _chanceTime,
                            new Label {Text = "Random events time (sec)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _moveChance,
                            new Label {Text = "Chance for brick move (%)", VerticalAlignment = VerticalAlignment.Center}
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
                                Text = "Chance for new brick spawn (%)",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _step,
                            new Label {Text = "Move step (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _size,
                            new Label {Text = "Brick size (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _brickColor,
                            new Label {Text = "Brick color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    }
                }
            })
            {
                Text = "Brick"
            };
        }
    }
}