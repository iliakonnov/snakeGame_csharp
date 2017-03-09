using System;
using System.IO;
using Eto.Forms;
using snake_game.Launcher.Config;
using snake_game.MainGame;

namespace snake_game.Launcher
{
    public class LauncherForm : Form
    {
        MainGame.Config _config = new MainGame.Config();
        void SaveHandler(object sender, EventArgs e)
        {
            File.WriteAllText("config.json", ConfigLoad.Save(_config));
        }

        void StartHandler(object sender, EventArgs e)
        {
            var game = new MainGame.MainGame(_config);
            game.Run();
        }

        void ResetHandler(object sender, EventArgs e)
        {
            _config = new MainGame.Config();
        }

        public LauncherForm()
        {
            _config = ConfigLoad.Parse(File.ReadAllText("config.json"));

            var gameCfg = new GameConfig(_config.GameConfig);
            var screenCfg = new ScreenConfig(_config.ScreenConfig);
            var snakeCfg = new SnakeConfig(_config.SnakeConfig);
            var bonusCfg = new BonusConfig(_config.BonusConfig);

            var saveButton = new Button {Text = "Save config"};
            saveButton.Click += SaveHandler;
            var startButton = new Button {Text = "Start!"};
            startButton.Click += StartHandler;
            var resetButton = new Button {Text = "Reset to default"};
            resetButton.Click += ResetHandler;

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