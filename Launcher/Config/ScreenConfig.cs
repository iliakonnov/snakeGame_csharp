using Eto.Forms;

namespace Launcher.Config
{
    /// <inheritdoc />
    /// <summary>
    ///     Все настройки, которые касаются отображения игры на экран
    /// </summary>
    public class ScreenConfig : IGameConfigPage<snake_game.MainGame.Config.ScreenConfigClass>
    {
        private readonly snake_game.MainGame.Config.ScreenConfigClass _config;
        private CheckBox _fullScreen;
        private NumericUpDown _height;
        private CheckBox _mouseVisible;
        private NumericUpDown _width;

        /// <inheritdoc />
        public ScreenConfig(snake_game.MainGame.Config.ScreenConfigClass config)
        {
            _config = config;
        }

        /// <inheritdoc />
        public snake_game.MainGame.Config.ScreenConfigClass GetConfig()
        {
            return new snake_game.MainGame.Config.ScreenConfigClass
            {
                IsMouseVisible = _mouseVisible.Checked ?? false,
                IsFullScreen = _fullScreen.Checked ?? false,
                ScreenWidth = (int) _width.Value,
                ScreenHeight = (int) _height.Value
            };
        }

        /// <inheritdoc />
        public TabPage GetPage()
        {
            _mouseVisible = new CheckBox {Checked = _config.IsMouseVisible};
            _fullScreen = new CheckBox {Checked = _config.IsFullScreen};
            _width = new NumericUpDown {Value = _config.ScreenWidth};
            _height = new NumericUpDown {Value = _config.ScreenHeight};

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _mouseVisible,
                            new Label {Text = "Mouse visible", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _fullScreen,
                            new Label {Text = "Full screen", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _width,
                            new Label {Text = "Screen width (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _height,
                            new Label {Text = "Screen height (px)", VerticalAlignment = VerticalAlignment.Center}
                        }
                    }
                }
            })
            {
                Text = "Screen"
            };
        }
    }
}