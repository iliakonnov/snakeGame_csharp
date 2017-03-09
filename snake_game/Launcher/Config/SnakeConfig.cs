using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class SnakeConfig
    {
        MainGame.Config _config;
        public SnakeConfig(MainGame.Config config)
        {
            _config = config;
        }

        public TabPage GetPage()
        {
            return new TabPage(new StackLayout
            {
                Items =
                {
                    new Label { Text = "SnakeConfig" }
                }
            })
            {
                Text = "SnakeConfig"
            };
        }
    }
}