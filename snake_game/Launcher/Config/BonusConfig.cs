using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using snake_game.Launcher.Bonuses;

namespace snake_game.Launcher.Config
{
    public class BonusConfig
    {
        CheckBox _enableBonuses;
        AppleConfig _apple;
        BrickConfig _brick;
        AntiBrickConfig _antiBrick;
        AntiAppleConfig _antiApple;
        MainGame.Config.BonusConfigClass _config;

        public BonusConfig(MainGame.Config.BonusConfigClass config)
        {
            _config = config;
        }

        public MainGame.Config.BonusConfigClass GetConfig()
        {
            var bonusesEnabled = new List<string>();
            if (_apple.IsEnabled()) bonusesEnabled.Add("apple");
            if (_brick.IsEnabled()) bonusesEnabled.Add("brick");
            if (_antiBrick.IsEnabled()) bonusesEnabled.Add("antibrick");
            if (_antiApple.IsEnabled()) bonusesEnabled.Add("antiapple");

            return new MainGame.Config.BonusConfigClass
            {
                BonusSettings = new MainGame.Config.BonusConfigClass.BonusSettingsClass
                {
                    EnableBonuses = _enableBonuses.Checked ?? false,
                    BonusesEnabled = bonusesEnabled.ToArray()
                },
                BrickConfig = _brick.GetConfig(),
                AppleConfig = _apple.GetConfig(),
                AntiBrickConfig = _antiBrick.GetConfig(),
                AntiAppleConfig = _antiApple.GetConfig()
            };
        }

        public TabPage GetPage(Eto.Drawing.Size size, int height)
        {
            _enableBonuses = new CheckBox
            {
                Height = height,
                Checked = _config.BonusSettings.EnableBonuses
            };
            if (_config.BonusSettings.BonusesEnabled == null)
            {
                _apple = new AppleConfig(_config.AppleConfig, true);
                _brick = new BrickConfig(_config.BrickConfig, true);
                _antiBrick = new AntiBrickConfig(_config.AntiBrickConfig, true);
                _antiApple = new AntiAppleConfig(_config.AntiAppleConfig, true);
            }
            else
            {
                _apple = new AppleConfig(_config.AppleConfig, _config.BonusSettings.BonusesEnabled.Contains("apple"));
                _brick = new BrickConfig(_config.BrickConfig, _config.BonusSettings.BonusesEnabled.Contains("brick"));
                _antiBrick = new AntiBrickConfig(_config.AntiBrickConfig,
                    _config.BonusSettings.BonusesEnabled.Contains("antibrick"));
                _antiApple = new AntiAppleConfig(_config.AntiAppleConfig,
                    _config.BonusSettings.BonusesEnabled.Contains("antiapple"));
            }

            return new TabPage(new StackLayout
            {
                Items =
                {
                    new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            _enableBonuses,
                            new Label {Text = "Enable bonuses", VerticalAlignment = VerticalAlignment.Center}
                        }
                    },
                    new TabControl
                    {
                        Size = new Eto.Drawing.Size(size.Width, size.Height - height),
                        Pages =
                        {
                            _brick.GetPage(),
                            _apple.GetPage(),
                            _antiBrick.GetPage(),
                            _antiApple.GetPage()
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