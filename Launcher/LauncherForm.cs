using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Launcher.Config;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace Launcher
{
    /// <inheritdoc />
    /// <summary>
    ///     Основное окно настроек
    /// </summary>
    public class LauncherForm : Form
    {
        private const int WindowWidth = 400;
        private const int WindowHeight = 400;
        private const int ButtonHeight = 30;
        private readonly Dictionary<string, IPluginContainer> _plugins;
        private IGameConfigPage<Dictionary<string, IPluginConfig>> _bonusCfg;

        private snake_game.MainGame.Config _config;

        private IGameConfigPage<snake_game.MainGame.Config.GameConfigClass> _gameCfg;
        private IGameConfigPage<snake_game.MainGame.Config.ScreenConfigClass> _screenCfg;

        /// <inheritdoc />
        public LauncherForm()
        {
            Title = "Snake Game Launcher";
            ClientSize = new Size(WindowWidth, WindowHeight);
            _plugins = BonusLoader.LoadPlugins(".");
            _config = File.Exists("config.json")
                ? ConfigLoad.Parse(File.ReadAllText("config.json"), _plugins)
                : new snake_game.MainGame.Config();
            Draw();
        }

        private void SaveHandler(object sender, EventArgs e)
        {
            _config = GetConfig();
            File.WriteAllText("config.json", ConfigLoad.Save(_config, _plugins));
        }

        private void StartHandler(object sender, EventArgs e)
        {
            SaveHandler(sender, e);
            Process.Start("snake_game.exe");
            Close();
        }

        private void ResetHandler(object sender, EventArgs e)
        {
            _config = new snake_game.MainGame.Config();
            Draw();
        }

        private snake_game.MainGame.Config GetConfig()
        {
            return new snake_game.MainGame.Config
            {
                ScreenConfig = _screenCfg.GetConfig(),
                GameConfig = _gameCfg.GetConfig(),
                BonusConfig = _bonusCfg.GetConfig()
            };
        }

        private void Draw()
        {
            _gameCfg = new GameConfig(_config.GameConfig);
            _screenCfg = new ScreenConfig(_config.ScreenConfig);
            _bonusCfg = new BonusConfig(_config.BonusConfig, _plugins.ToDictionary(x => x.Key, x => x.Value.Plugin));

            var saveButton = new Button
            {
                Text = "Save config",
                Size = new Size(WindowWidth / 3, ButtonHeight)
            };
            saveButton.Click += SaveHandler;
            var startButton = new Button
            {
                Text = "Start!",
                Size = new Size(WindowWidth / 3, ButtonHeight)
            };
            startButton.Click += StartHandler;
            var resetButton = new Button
            {
                Text = "Reset to default",
                Size = new Size(WindowWidth / 3, ButtonHeight)
            };
            resetButton.Click += ResetHandler;

            var tabControlSize = new Size(WindowWidth, WindowHeight - ButtonHeight);

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
                            _bonusCfg.GetPage()
                        }
                    },
                    new StackLayout // Buttons
                    {
                        Size = new Size(WindowWidth, ButtonHeight
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