using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class ScreenConfig
    {
        MainGame.Config _config;
        public ScreenConfig(MainGame.Config config)
        {
            _config = config;
        }

        public TabPage GetPage()
        {
            return new TabPage(new StackLayout
            {
                Items =
                {
                    new Label { Text = "ScreenConfig" }
                }
            })
            {
                Text = "ScreenConfig"
            };
        }
    }
}