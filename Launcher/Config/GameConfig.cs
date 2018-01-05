using System;
using Eto.Forms;
using Microsoft.Xna.Framework;
using snake_game.Utils;

namespace Launcher.Config
{
    /// <inheritdoc />
    /// <summary>
    ///     Все настройки, которые касаются ядра игры
    /// </summary>
    public class GameConfig : IGameConfigPage<snake_game.MainGame.Config.GameConfigClass>
    {
        private readonly snake_game.MainGame.Config.GameConfigClass _config;
        private ColorPicker _bgColor;
        private ColorPicker _fogColor;
        private CheckBox _fogEnabled;
        private NumericUpDown _fogSize;
        private NumericUpDown _foodToLive;
        private NumericUpDown _lives;
        private ColorPicker _textColor;

        /// <inheritdoc />
        public GameConfig(snake_game.MainGame.Config.GameConfigClass config)
        {
            _config = config;
        }

        /// <inheritdoc />
        public snake_game.MainGame.Config.GameConfigClass GetConfig()
        {
            return new snake_game.MainGame.Config.GameConfigClass
            {
                Lives = (int) _lives.Value,
                ScoreToLive = (int) _foodToLive.Value,
                TextColor = ColorConverter.ToXna(_textColor.Value),
                FogEnabled = _fogEnabled.Checked ?? false,
                FogColor = new Tuple<Color, Color>(ColorConverter.ToXna(_fogColor.Value), Color.Transparent),
                FogSize = _fogSize.Value,
                BackgroundColor = ColorConverter.ToXna(_bgColor.Value)
            };
        }

        /// <inheritdoc />
        public TabPage GetPage()
        {
            _lives = new NumericUpDown {Value = _config.Lives};
            _foodToLive = new NumericUpDown {Value = _config.ScoreToLive};
            _textColor = new ColorPicker {Value = ColorConverter.ToEto(_config.TextColor)};
            _bgColor = new ColorPicker {Value = ColorConverter.ToEto(_config.BackgroundColor)};
            _fogColor = new ColorPicker {Value = ColorConverter.ToEto(_config.FogColor.Item1)};
            _fogEnabled = new CheckBox {Checked = _config.FogEnabled};
            _fogSize = new NumericUpDown {Value = _config.FogSize};


            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = {_lives, new Label {Text = "Lives", VerticalAlignment = VerticalAlignment.Center}}
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _foodToLive,
                            new Label {Text = "Food to get extra live", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _textColor,
                            new Label {Text = "Information text color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _bgColor,
                            new Label {Text = "Background color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogColor,
                            new Label {Text = "Fog color", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new Label {Height = 2}, // Separator
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogEnabled,
                            new Label {Text = "Enable fog", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fogSize,
                            new Label {Text = "Fog size", VerticalAlignment = VerticalAlignment.Center}
                        }
                    }
                }
            })
            {
                Text = "Game"
            };
        }
    }
}