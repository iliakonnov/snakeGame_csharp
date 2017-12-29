using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using System;
using System.Runtime.InteropServices;
using MonoGame.Extended;
using QuakeConsole;
using snake_game.Bonuses;
using snake_game.Utils;

namespace snake_game.MainGame
{
    public partial class MainGame : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteFont _font;
        private Fog _fog;
        public BagelWorld World;

        // ReSharper disable MemberCanBePrivate.Global
        // For Quake Console
        public BonusManager BonusManager;
        public SpriteBatch SpriteBatch;
        public int _score;
        public int DamagedTime;
        public int GameTime;
        public int Lives;
        public readonly Config Config;
        public readonly Dictionary<string, IPlugin> Plugins;
        // ReSharper restore MemberCanBePrivate.Global
        private readonly QuakeConsole.ConsoleComponent _console;
        private readonly object _exitLock = new object();
        private bool _exited = false;

        public MainGame(Config config, Dictionary<string, IPlugin> plugins)
        {
            _graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = config.ScreenConfig.IsMouseVisible;
            _graphics.IsFullScreen = config.ScreenConfig.IsFullScreen;
            _graphics.PreferredBackBufferHeight = config.ScreenConfig.ScreenHeight;
            _graphics.PreferredBackBufferWidth = config.ScreenConfig.ScreenWidth;

            Plugins = plugins;
            Config = config;
            Lives = Config.GameConfig.Lives;
            DamagedTime = -Config.GameConfig.DamageTimeout;
            
            _console = new ConsoleComponent(this);
            Components.Add(_console);

            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            var interpreter = new PythonInterpreter();
            _console.Interpreter = interpreter;
            interpreter.AddVariable("game", this, 100);
            
            _fog = new Fog(GraphicsDevice, Config.GameConfig.FogColor.Item1, Config.GameConfig.FogColor.Item2);

            _font = Content.Load<SpriteFont>("DejaVu Sans Mono");

            var seed = DateTime.Now.Millisecond;
            BonusManager = new BonusManager(Config.BonusConfig, Plugins, this, new Random(seed));
            BonusManager.LoadContent(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_exited || Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0)
            {
                base.Update(gameTime);
                return;
            }

            GameTime += gameTime.ElapsedGameTime.Milliseconds;

            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.OemTilde))
            {
                _console.ToggleOpenClose();
            }
            else if (!_console.IsVisible)
            {
                if (Lives <= 0 || keyState.IsKeyDown(Keys.Escape))
                {
                    lock (_exitLock)
                        Exit();
                    _exited = true;
                    base.Update(gameTime);
                    return;
                }

                World = new BagelWorld(Window.ClientBounds.Height, Window.ClientBounds.Width);

                var gameEvents = new GameEvents
                {
                    Invulnerable = GameTime - DamagedTime <= Config.GameConfig.DamageTimeout,
                    Damage = _damage
                };
                BonusManager.Update(gameTime, GameTime, keyState, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), gameEvents);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_exited || Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0)
            {
                base.Draw(gameTime);
                return;
            }
            lock (_exitLock)
            {
                if (Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0)
                {
                    return;
                }
                
                var fogDistance = Config.GameConfig.FogSize;

                _graphics.GraphicsDevice.Clear(Config.GameConfig.BackgroundColor);
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                SpriteBatch.DrawString(
                    _font, $"Score: {_score}\nLives: {Lives}",
                    new Vector2(
                        (int) Math.Ceiling(fogDistance),
                        (int) Math.Ceiling(fogDistance)
                    ), Config.GameConfig.TextColor
                );

                BonusManager.Draw(SpriteBatch);

                if (Config.GameConfig.FogEnabled)
                    _fog.CreateFog(SpriteBatch,
                        new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                        (int) Math.Round(fogDistance));

                SpriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}