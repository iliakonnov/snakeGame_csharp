using Eto.Forms;

namespace snake_game.Launcher.Bonuses
{
    public class BrickConfig
    {
        CheckBox _enabled;
        NumericUpDown _chanceTime;
        NumericUpDown _moveChance;
        NumericUpDown _newChance;
        NumericUpDown _step;
        NumericUpDown _size;
        ColorPicker _brickColor;

        MainGame.Config.BonusConfigClass.BrickConfigClass _config;
        public BrickConfig(MainGame.Config.BonusConfigClass.BrickConfigClass config, bool enabled)
        {
            _enabled = new CheckBox {Checked = enabled};
            _config = config;
        }

        public MainGame.Config.BonusConfigClass.BrickConfigClass GetConfig()
        {
            return new MainGame.Config.BonusConfigClass.BrickConfigClass
            {
                ChanceTime = (int) _chanceTime.Value*1000,
                MoveChance = _moveChance.Value / 100,
                NewChance = _newChance.Value / 100,
                Step = (int) _step.Value * 2,
                BrickColor = ColorConverter.ToXna(_brickColor.Value),
                Size = (int) _size.Value
            };
        }

        public bool IsEnabled()
        {
            return _enabled.Checked ?? false;
        }

        public TabPage GetPage()
        {
            _chanceTime = new NumericUpDown {Value = _config.ChanceTime / 1000};
            _moveChance = new NumericUpDown {Value = _config.MoveChance * 100};
            _newChance = new NumericUpDown {Value = _config.NewChance * 100};
            _step = new NumericUpDown {Value = _config.Step / 2};
            _size = new NumericUpDown {Value = _config.Size};
            _brickColor = new ColorPicker {Value = ColorConverter.ToEto(_config.BrickColor)};

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _enabled, new Label { Text = "Bonus enabled"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _chanceTime, new Label { Text = "Random events time (sec)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _moveChance, new Label { Text = "Chance for brick move (%)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _newChance, new Label { Text = "Chance for new brick spawn (%)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _step, new Label { Text = "Move step (px)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _size, new Label { Text = "Brick size (px)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _brickColor, new Label { Text = "Brick color"} }
                    }
                }
            })
            {
                Text = "Brick"
            };
        }
    }
}