using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using System;
using snake_game.Bonuses;

namespace snake_game.MainGame
{
    public partial class MainGame : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        public BagelWorld World;
        private Fog _fog;
        private BonusManager _bonusManager;
        private int _score;
        private int _damagedTime;
        private int _gameTime;
        private int _lives;
        private readonly Config _config;
        private readonly Dictionary<string, IPlugin> _plugins;
        private readonly object _exitLock = new object();
        private bool _exited = false;

        public MainGame(Config config, Dictionary<string, IPlugin> plugins)
        {
            _graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = config.ScreenConfig.IsMouseVisible;
            _graphics.IsFullScreen = config.ScreenConfig.IsFullScreen;
            _graphics.PreferredBackBufferHeight = config.ScreenConfig.ScreenHeight;
            _graphics.PreferredBackBufferWidth = config.ScreenConfig.ScreenWidth;

            _plugins = plugins;
            _config = config;
            _lives = _config.GameConfig.Lives;
            _damagedTime = -_config.GameConfig.DamageTimeout;

            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fog = new Fog(GraphicsDevice, _config.GameConfig.FogColor.Item1, _config.GameConfig.FogColor.Item2);

            _font = Content.Load<SpriteFont>("DejaVu Sans Mono");

            var seed = DateTime.Now.Millisecond;
            _bonusManager = new BonusManager(_config.BonusConfig, _plugins, this, new Random(seed));
            _bonusManager.LoadContent(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_exited)
            {
                base.Update(gameTime);
                return;
            }

            _gameTime += gameTime.ElapsedGameTime.Milliseconds;

            var keyState = Keyboard.GetState();

            if (_lives <= 0 || keyState.IsKeyDown(Keys.Escape))
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
                Invulnerable = _gameTime - _damagedTime <= _config.GameConfig.DamageTimeout,
                Damage = _damage
            };
            _bonusManager.Update(gameTime, _gameTime, keyState, Window.ClientBounds, gameEvents);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_exited)
            {
                base.Draw(gameTime);
                return;
            }
            lock (_exitLock)
            {
                var fogDistance = _config.GameConfig.FogSize;

                _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                _spriteBatch.DrawString(
                    _font, $"Score: {_score}\nLives: {_lives}",
                    new Vector2(
                        (int) Math.Ceiling(fogDistance),
                        (int) Math.Ceiling(fogDistance)
                    ), _config.GameConfig.TextColor
                );

                _bonusManager.Draw(_spriteBatch);

                if (_config.GameConfig.FogEnabled)
                    _fog.CreateFog(_spriteBatch,
                        new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                        (int) Math.Round(fogDistance));

                _spriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}