using System;
using System.IO;
using Eto.Forms;
using System.Threading;
using snake_game.Launcher.Config;
using snake_game.MainGame;

namespace snake_game.Launcher
{
    public class LauncherForm : Form
    {
        GameConfig _gameCfg;
        ScreenConfig _screenCfg;
        SnakeConfig _snakeCfg;
        BonusConfig _bonusCfg;

        MainGame.Config _config;
        void SaveHandler(object sender, EventArgs e)
        {
            _config = GetConfig();
            File.WriteAllText("config.json", ConfigLoad.Save(_config));
        }

        void StartHandler(object sender, EventArgs e)
        {
            _config = GetConfig();
            new Thread(() =>
            {
                var game = new MainGame.MainGame(_config);
                game.Run();
            }).Start();
            Close();
        }

        void ResetHandler(object sender, EventArgs e)
        {
            _config = new MainGame.Config();
            Draw();
        }

        MainGame.Config GetConfig()
        {
            return new MainGame.Config
            {
                SnakeConfig = _snakeCfg.GetConfig(),
                ScreenConfig = _screenCfg.GetConfig(),
                GameConfig = _gameCfg.GetConfig(),
                BonusConfig = _bonusCfg.GetConfig()
            };
        }

        public LauncherForm()
        {
            _config = ConfigLoad.Parse(File.ReadAllText("config.json"));
            Draw();
        }

        void Draw()
        {
            _gameCfg = new GameConfig(_config.GameConfig);
            _screenCfg = new ScreenConfig(_config.ScreenConfig);
            _snakeCfg = new SnakeConfig(_config.SnakeConfig);
            _bonusCfg = new BonusConfig(_config.BonusConfig);

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
                            _gameCfg.GetPage(),
                            _screenCfg.GetPage(),
                            _snakeCfg.GetPage(),
                            _bonusCfg.GetPage()
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