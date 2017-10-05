using System;
using System.Collections.Generic;
using System.IO;
using Eto.Forms;
using System.Threading;
using snake_game.Bonuses;
using snake_game.Launcher.Config;
using snake_game.MainGame;

namespace snake_game.Launcher
{
    public class LauncherForm : Form
    {
        const int WindowWidth = 400;
        const int WindowHeight = 400;
        const int ButtonHeight = 30;

        GameConfig _gameCfg;
        ScreenConfig _screenCfg;
        SnakeConfig _snakeCfg;
        BonusConfig _bonusCfg;

        MainGame.Config _config;
        Dictionary<string, IPlugin> _plugins;

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
                var game = new MainGame.MainGame(_config, _plugins);
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
            ClientSize = new Eto.Drawing.Size(WindowWidth, WindowHeight);
            _config = File.Exists("config.json")
                ? ConfigLoad.Parse(File.ReadAllText("config.json"))
                : new MainGame.Config();
            _plugins = BonusLoader.LoadPlugins("plugins");
            Draw();
        }

        void Draw()
        {
            _gameCfg = new GameConfig(_config.GameConfig);
            _screenCfg = new ScreenConfig(_config.ScreenConfig);
            _snakeCfg = new SnakeConfig(_config.SnakeConfig);
            _bonusCfg = new BonusConfig(_config.BonusConfig, _plugins);

            var saveButton = new Button
            {
                Text = "Save config",
                Size = new Eto.Drawing.Size(WindowWidth / 3, ButtonHeight)
            };
            saveButton.Click += SaveHandler;
            var startButton = new Button
            {
                Text = "Start!",
                Size = new Eto.Drawing.Size(WindowWidth / 3, ButtonHeight)
            };
            startButton.Click += StartHandler;
            var resetButton = new Button
            {
                Text = "Reset to default",
                Size = new Eto.Drawing.Size(WindowWidth / 3, ButtonHeight)
            };
            resetButton.Click += ResetHandler;

            var tabControlSize = new Eto.Drawing.Size(WindowWidth, WindowHeight - ButtonHeight);

            Content = new StackLayout
            {
                Items =
                {
                    new TabControl
                    {
                        Size = tabControlSize,
                        Pages =
                        {
                            _gameCfg.GetPage(),
                            _screenCfg.GetPage(),
                            _snakeCfg.GetPage(),
                            _bonusCfg.GetPage(tabControlSize, ButtonHeight)
                        }
                    },
                    new StackLayout // Buttons
                    {
                        Size = new Eto.Drawing.Size(WindowWidth, ButtonHeight
                        ),
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