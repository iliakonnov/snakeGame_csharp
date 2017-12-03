using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Eto.Forms;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

        private GameConfig _gameCfg;
        private ScreenConfig _screenCfg;
        private BonusConfig _bonusCfg;

        private MainGame.Config _config;
        readonly Dictionary<string, IPlugin> _plugins;

        private void SaveHandler(object sender, EventArgs e)
        {
            _config = GetConfig();
            File.WriteAllText("config.json", ConfigLoad.Save(_config));
        }

        private void StartHandler(object sender, EventArgs e)
        {
            _config = GetConfig();
            new MainGame.MainGame(_config, _plugins).Run();
            Close();
        }

        private void ResetHandler(object sender, EventArgs e)
        {
            _config = new MainGame.Config();
            Draw();
        }

        private MainGame.Config GetConfig()
        {
            return new MainGame.Config
            {
                ScreenConfig = _screenCfg.GetConfig(),
                GameConfig = _gameCfg.GetConfig(),
                BonusConfig = _bonusCfg.GetConfig()
            };
        }

        public LauncherForm()
        {
            ClientSize = new Eto.Drawing.Size(WindowWidth, WindowHeight);
            _plugins = BonusLoader.LoadPlugins(".");
            _config = File.Exists("config.json")
                ? ConfigLoad.Parse(File.ReadAllText("config.json"), _plugins.Values.Select(p=>p.GetType().Assembly).ToArray())
                : new MainGame.Config();
            Draw();
        }

        private void Draw()
        {
            _gameCfg = new GameConfig(_config.GameConfig);
            _screenCfg = new ScreenConfig(_config.ScreenConfig);
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
                            _bonusCfg.GetPage(tabControlSize)
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