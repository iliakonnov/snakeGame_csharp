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
		const int WINDOW_WIDTH = 500;
		const int WINDOW_HEIGHT = 300;
		const int BUTTON_HEIGHT = 30;

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
			ClientSize = new Eto.Drawing.Size(WINDOW_WIDTH, WINDOW_HEIGHT);
            _config = File.Exists("config.json")
                ? ConfigLoad.Parse(File.ReadAllText("config.json"))
                : new MainGame.Config();
            Draw();
        }

        void Draw()
        {
            _gameCfg = new GameConfig(_config.GameConfig);
            _screenCfg = new ScreenConfig(_config.ScreenConfig);
            _snakeCfg = new SnakeConfig(_config.SnakeConfig);
            _bonusCfg = new BonusConfig(_config.BonusConfig);

            var saveButton = new Button {
				Text = "Save config",
				Size = new Eto.Drawing.Size(WINDOW_WIDTH / 3, BUTTON_HEIGHT)
			};
            saveButton.Click += SaveHandler;
            var startButton = new Button {
				Text = "Start!",
				Size = new Eto.Drawing.Size(WINDOW_WIDTH / 3, BUTTON_HEIGHT)
			};
            startButton.Click += StartHandler;
            var resetButton = new Button {
				Text = "Reset to default",
				Size = new Eto.Drawing.Size(WINDOW_WIDTH / 3, BUTTON_HEIGHT)
			};
            resetButton.Click += ResetHandler;

			var tabControlSize = new Eto.Drawing.Size(WINDOW_WIDTH, WINDOW_HEIGHT - BUTTON_HEIGHT);

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
                            _bonusCfg.GetPage(tabControlSize, BUTTON_HEIGHT)
                        }
                    },
                    new StackLayout  // Buttons
                    {
						Size = new Eto.Drawing.Size(WINDOW_WIDTH, BUTTON_HEIGHT
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