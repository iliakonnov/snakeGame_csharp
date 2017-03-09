using System;
using Eto.Forms;
using snake_game.Launcher.Config;

namespace snake_game.Launcher
{
    public class LauncherForm : Form
    {
        MainGame.Config _config = new MainGame.Config();
        public void SaveHandler(object sender, EventArgs e)
        {
        }

        public void StartHandler(object sender, EventArgs e)
        {
        }

        public LauncherForm()
        {
            var gameCfg = new GameConfig(_config.GameConfig);
            var screenCfg = new ScreenConfig(_config);
            var snakeCfg = new SnakeConfig(_config);
            var bonusCfg = new BonusConfig(_config);

            var saveButton = new Button {Text = "Save config"};
            saveButton.Click += SaveHandler;
            var startButton = new Button {Text = "Start!"};
            saveButton.Click += StartHandler;

            Content = new StackLayout
            {
                Items =
                {
                    new TabControl
                    {
                        Pages =
                        {
                            gameCfg.GetPage(),
                            screenCfg.GetPage(),
                            snakeCfg.GetPage(),
                            bonusCfg.GetPage()
                        }
                    },
                    new StackLayout  // Buttons
                    {
                        Orientation = Orientation.Horizontal,
                        Items =
                        {
                            saveButton,
                            startButton
                        }
                    }
                }
            };
        }
    }
}