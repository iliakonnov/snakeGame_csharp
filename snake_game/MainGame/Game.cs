using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QuakeConsole;
using snake_game.Bonuses;

namespace snake_game.MainGame
{
    /// <summary>
    ///     Класс ядра ягры
    /// </summary>
    public class MainGame : Game
    {
        private readonly ConsoleComponent _console;
        private readonly object _exitLock = new object();
        private readonly GraphicsDeviceManager _graphics;
        private bool _exited;
        private SpriteFont _font;

        /// <summary>
        ///     Игровой мир
        /// </summary>
        public BagelWorld World;

        /// <inheritdoc />
        /// <param name="config">Параметры ядра игры</param>
        /// <param name="plugins">Загруженные плагины</param>
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

            _console = new ConsoleComponent(this);
            Components.Add(_console);

            Content.RootDirectory = "Content";
        }

        /// <inheritdoc />
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            var interpreter = new PythonInterpreter();
            _console.Interpreter = interpreter;
            interpreter.AddVariable("game", this, 100);

            Fog = new Fog(GraphicsDevice, Config.GameConfig.FogColor.Item1, Config.GameConfig.FogColor.Item2);

            _font = Content.Load<SpriteFont>("DejaVu Sans Mono");

            var seed = DateTime.Now.Millisecond;
            BonusManager = new BonusManager(Config.BonusConfig, Plugins, this, new Random(seed));
            BonusManager.LoadContent(GraphicsDevice);

            base.LoadContent();
        }

        /// <inheritdoc />
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
                    {
                        Exit();
                    }

                    _exited = true;
                    base.Update(gameTime);
                    return;
                }

                World = new BagelWorld(Window.ClientBounds.Height, Window.ClientBounds.Width);

                var gameEvents = new GameEvents
                {
                    Damage = DamageCount
                };
                BonusManager.Update(gameTime, GameTime, keyState,
                    new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), gameEvents);
            }

            base.Update(gameTime);
        }

        /// <inheritdoc />
        protected override void Draw(GameTime gameTime)
        {
            if (_exited || Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0)
            {
                base.Draw(gameTime);
                return;
            }

            lock (_exitLock)
            {
                if (Window.ClientBounds.Width == 0 || Window.ClientBounds.Height == 0) return;

                var fogDistance = Config.GameConfig.FogSize;

                _graphics.GraphicsDevice.Clear(Config.GameConfig.BackgroundColor);
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                SpriteBatch.DrawString(
                    _font, $"Score: {ScoreCount}\nLives: {Lives}",
                    new Vector2(
                        (int) Math.Ceiling(fogDistance),
                        (int) Math.Ceiling(fogDistance)
                    ), Config.GameConfig.TextColor
                );

                BonusManager.Draw(SpriteBatch);

                if (Config.GameConfig.FogEnabled)
                    Fog.CreateFog(SpriteBatch,
                        new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                        (int) Math.Round(fogDistance));

                SpriteBatch.End();
                base.Draw(gameTime);
            }
        }

        /// <summary>
        ///     Уменьшает кол-во жизней змеи
        /// </summary>
        /// <param name="damage">На сколько уменьшить. Может быть &lt; 0</param>
        public void Damage(int damage)
        {
            Lives -= damage;
            DamageCount = damage;
        }

        /// <summary>
        ///     Увеличивает игровой счет
        /// </summary>
        /// <param name="score">На сколько увеличить.</param>
        public void Score(int score)
        {
            ScoreCount += score;
            if (ScoreCount % Config.GameConfig.ScoreToLive == 0) Lives += 1;
        }

        #region Public for Quake Console

        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        ///     Туман по краям карты
        /// </summary>
        public Fog Fog;

        /// <summary>
        ///     Управляет всеми бонусами
        /// </summary>
        public BonusManager BonusManager;

        /// <summary>
        ///     Необходимо для отрисовки на экран
        /// </summary>
        public SpriteBatch SpriteBatch;

        /// <summary>
        ///     Текущий игровой счет
        /// </summary>
        public int ScoreCount;

        /// <summary>
        ///     Количество нанесенного урона за тик
        /// </summary>
        public int DamageCount;

        /// <summary>
        ///     Суммарное игровое время
        /// </summary>
        public int GameTime;

        /// <summary>
        ///     Количество оставшихся жизней
        /// </summary>
        public int Lives;

        /// <summary>
        ///     Параметры ядра игры
        /// </summary>
        public readonly Config Config;

        /// <summary>
        ///     Загруженные плагины
        /// </summary>
        public readonly Dictionary<string, IPlugin> Plugins;
        // ReSharper restore MemberCanBePrivate.Global

        #endregion
    }
}