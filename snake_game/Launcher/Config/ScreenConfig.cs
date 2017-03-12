using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class ScreenConfig
    {
        CheckBox _mouseVisible;
        CheckBox _fullScreen;
        NumericUpDown _width;
        NumericUpDown _height;

        MainGame.Config.ScreenConfigClass _config;
        public ScreenConfig(MainGame.Config.ScreenConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.ScreenConfigClass GetConfig()
        {
            return new MainGame.Config.ScreenConfigClass
            {
                IsMouseVisible = _mouseVisible.Checked ?? false,
                IsFullScreen = _fullScreen.Checked ?? false,
                ScreenWidth = (int) _width.Value,
                ScreenHeight = (int) _height.Value
            };
        }

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
                        Items = { _mouseVisible, new Label { Text = "Mouse visible", VerticalAlignment = VerticalAlignment.Center } }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _fullScreen, new Label { Text = "Full screen", VerticalAlignment = VerticalAlignment.Center } }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _width, new Label { Text = "Screen width (px)", VerticalAlignment = VerticalAlignment.Center } }
                    },
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _height, new Label { Text = "Screen height (px)", VerticalAlignment = VerticalAlignment.Center } }
                    }
                }
            })
            {
                Text = "Screen"
            };
        }
    }
}