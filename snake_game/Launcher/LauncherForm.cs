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

        public void ResetHandler(object sender, EventArgs e)
        {
        }

        public LauncherForm()
        {
            var gameCfg = new GameConfig(_config.GameConfig);
            var screenCfg = new ScreenConfig(_config.ScreenConfig);
            var snakeCfg = new SnakeConfig(_config.SnakeConfig);
            var bonusCfg = new BonusConfig(_config.BonusConfig);

            var saveButton = new Button {Text = "Save config"};
            saveButton.Click += SaveHandler;
            var startButton = new Button {Text = "Start!"};
            saveButton.Click += StartHandler;
            var resetButton = new Button {Text = "Reset to default"};
            saveButton.Click += ResetHandler;

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
                            resetButton,
                            saveButton,
                            startButton
                        }
                    }
                }
            };
        }
    }
}