using Eto.Forms;

namespace snake_game.Launcher.Config
{
    public class BonusConfig
    {
        MainGame.Config _config;
        public BonusConfig(MainGame.Config config)
        {
            _config = config;
        }

        public TabPage GetPage()
        {
            return new TabPage(new StackLayout
            {
                Items =
                {
                    new Label { Text = "BonusConfig" }
                }
            })
            {
                Text = "BonusConfig"
            };
        }
    }
}