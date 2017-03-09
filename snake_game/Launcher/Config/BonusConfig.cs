using Eto.Forms;
using snake_game.Launcher.Bonuses;

namespace snake_game.Launcher.Config
{
    public class BonusConfig
    {
        CheckBox _enableBonuses;
        AppleConfig _apple;
        BrickConfig _brick;
        MainGame.Config.BonusConfigClass _config;

        public BonusConfig(MainGame.Config.BonusConfigClass config)
        {
            _config = config;
        }

        public TabPage GetPage()
        {
            _enableBonuses = new CheckBox {Checked = _config.BonusSettings.EnableBonuses};
            _apple = new AppleConfig(_config.AppleConfig);
            _brick = new BrickConfig(_config.BrickConfig);

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items = { _enableBonuses, new Label { Text = "Enable bonuses"} }
                    },
                    new TabControl
                    {
                        Pages =
                        {
                            _brick.GetPage(),
                            _apple.GetPage(),
                        }
                    }
                }
            })
            {
                Text = "Bonuses"
            };
        }
    }
}