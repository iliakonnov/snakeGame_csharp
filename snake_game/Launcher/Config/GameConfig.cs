using Eto.Drawing;
using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class GameConfig
    {
        MainGame.Config.GameConfigClass _config;
        public GameConfig(MainGame.Config.GameConfigClass config)
        {
            _config = config;
        }

        public TabPage GetPage()
        {
            var lives = new NumericUpDown {Value = _config.Lives};
            var damageTimeout = new NumericUpDown {Value = _config.DamageTimeout};
            var foodToLive = new NumericUpDown {Value = _config.FoodToLive};
            var textColor = new ColorPicker {Value = ColorConverter.ToEto(_config.TextColor)};
            var bgColor = new ColorPicker {Value = ColorConverter.ToEto(_config.BackgroundColor)};
            var fogColor = new ColorPicker {Value = ColorConverter.ToEto(_config.FogColor.Item1)};
            var debugEnabled = new CheckBox {Checked = _config.DebugShow};
            var debugColor = new ColorPicker {Value = ColorConverter.ToEto(_config.DebugColor)};
            var fogEnabled = new CheckBox {Checked = _config.FogEnabled};
            var fogSizeMultiplier = new NumericUpDown {Value = _config.FogSizeMultiplier};

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { lives, new Label { Text = "Lives"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { damageTimeout, new Label { Text = "Damage timeout"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { foodToLive, new Label { Text = "Food to get extra live"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { textColor, new Label { Text = "Information text color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { bgColor, new Label { Text = "Background color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { fogColor, new Label { Text = "Fog color"} }
                    },
                    new Label { Height = 2 },  // Separator
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { debugEnabled, new Label { Text = "Enable debug (~)"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { debugColor, new Label { Text = "Debug window color"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { fogEnabled, new Label { Text = "Enable fog"} }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { fogSizeMultiplier, new Label { Text = "Fog size multiplier"} }
                    }
                }
            })
            {
                Text = "GameConfig"
            };
        }
    }
}