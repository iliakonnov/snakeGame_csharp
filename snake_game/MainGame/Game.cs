using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using snake_game.Snake;
using System;
using snake_game.Bonuses;

namespace snake_game.MainGame
{
    public partial class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteFont _font;
        SpriteBatch _spriteBatch;
        SnakeModel _snake;
        BagelWorld _world;
        IController _ctrl;
        Color[] _colors;
        Fog _fog;
        BonusManager _bonusManager;
        Logger _log;
        int _score;
        int _dieTime;
        int _gameTime;
        int _lives;
        int _intersectStart;
        readonly Debug _dbg;
        readonly Config _config;

        public MainGame(Config config)
        {
            _graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = config.ScreenConfig.IsMouseVisible;
            _graphics.IsFullScreen = config.ScreenConfig.IsFullScreen;
            _graphics.PreferredBackBufferHeight = config.ScreenConfig.ScreenHeight;
            _graphics.PreferredBackBufferWidth = config.ScreenConfig.ScreenWidth;

            _config = config;
            _lives = _config.GameConfig.Lives;
            _dieTime = -_config.GameConfig.DamageTimeout;

            Content.RootDirectory = "Content";

            _dbg = new Debug(this, _config.GameConfig.DebugColor);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fog = new Fog(GraphicsDevice, _config.GameConfig.FogColor.Item1, _config.GameConfig.FogColor.Item2);
            _snake =
                new SnakeModel(new Snake.Point(400, 150), 0).Increase(
                    _config.SnakeConfig.InitLen * _config.SnakeConfig.CircleOffset);
            _ctrl = new ControllerTraditional();
            _font = Content.Load<SpriteFont>("DejaVu Sans Mono");

            if (_config.SnakeConfig.Colors == null)
            {
                var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

                var colors = new List<Color>();
                if (_config.SnakeConfig.HeadColor != null) colors.Add((Color) _config.SnakeConfig.HeadColor);
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color))
                    {
                        var col = (Color) propertyInfo.GetValue(null, null);
                        if (col != _config.GameConfig.BackgroundColor && col != _config.SnakeConfig.HeadColor &&
                            col.A == 255)
                        {
                            colors.Add(col);
                        }
                    }
                }
                _colors = colors.ToArray();
            }
            else
            {
                _colors = _config.SnakeConfig.Colors;
            }

            var points = _snake.GetSnakeAsPoints(_config.SnakeConfig.CircleOffset);
            var headCenter = points.First();
            var head = new CircleF(
                new Vector2(headCenter.X, headCenter.Y), _config.SnakeConfig.CircleSize
            );

            var i = 1;
            bool intersects;
            do
            {
                var current = new CircleF(
                    new Vector2(points[i].X, points[i].Y), _config.SnakeConfig.CircleSize
                );
                i++;
                intersects = head.Intersects(current);
            } while (intersects);
            _intersectStart = i;

            var seed = DateTime.Now.Millisecond;
            _log = new Logger(seed, _config);
            _bonusManager = new BonusManager(_config.BonusConfig, this, new Random(seed));
            _bonusManager.LoadContent(GraphicsDevice);

            _dbg.LoadContent();
            _dbg.IsEnabled = _config.GameConfig.DebugShow;

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _gameTime += gameTime.ElapsedGameTime.Milliseconds;
            _dbg.Update(gameTime);
            var control = _ctrl.Control(Keyboard.GetState());
            _log.LogAction(_gameTime, control);

            if (control.Debug)
            {
                _dbg.IsEnabled = !_dbg.IsEnabled;
            }
            if (control.IsExit)
            {
                Exit();
                return;
            }
            if (control.Turn.ToTurn)
            {
                _snake = control.Turn.ReplaceTurn
                    ? _snake.TurnAt(control.Turn.TurnDegrees)
                    : _snake.Turn(control.Turn.TurnDegrees);
            }

            _snake = _snake.ContinueMove(_config.SnakeConfig.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            var halfSize = _config.SnakeConfig.CircleSize / 2;
            var newSize = _dbg.Size();
            var world = new BagelWorld(newSize.Height, newSize.Width);
            var points = _snake.GetSnakeAsPoints(_config.SnakeConfig.CircleOffset);
            var circles = new CircleF[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                var normPoint = world.Normalize(points[i]);
                circles[i] = new CircleF(new Vector2(normPoint.X, normPoint.Y), halfSize);
            }
            var head = circles.First();

            for (int i = _intersectStart; i < circles.Length; i++)
            {
                if (head.Intersects(circles[i]))
                {
                    Die(1);
                }
            }

            _bonusManager.Update(gameTime, _gameTime, circles, newSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var halfSize = _config.SnakeConfig.CircleSize / 2;
            var circle = CreateCircleTexture(_config.SnakeConfig.CircleSize);
            var newSize = _dbg.Size();
            _world = new BagelWorld(newSize.Height, newSize.Width);
            var snakePoints = _snake.GetSnakeAsPoints(_config.SnakeConfig.CircleOffset)
                                    .Select(x => _world.Normalize(x))
                                    .ToArray();
            var fogDistance = _config.SnakeConfig.CircleSize * _config.GameConfig.FogSizeMultiplier;

            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _dbg.Draw(snakePoints);

            _spriteBatch.DrawString(
                _font, $"Score: {_score}\nLives: {_lives}",
                new Vector2(
                    (int) Math.Ceiling(fogDistance),
                    (int) Math.Ceiling(fogDistance)
                ), _config.GameConfig.TextColor
            );

            for (var i = 0; i < snakePoints.Length; i++)
            {
                _spriteBatch.Draw(
                    circle,
                    new Vector2(
                        snakePoints[i].X - halfSize,
                        snakePoints[i].Y - halfSize
                    ),
                    _gameTime - _dieTime > _config.GameConfig.DamageTimeout
                        ? _colors[i % _colors.Length]
                        : _config.SnakeConfig.DamageColor
                );
            }
            _bonusManager.Draw(_spriteBatch);

            if (_config.GameConfig.FogEnabled) _fog.CreateFog(_spriteBatch, newSize, (int) Math.Round(fogDistance));

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        Texture2D CreateCircleTexture(int radius)
        {
            var texture = new Texture2D(GraphicsDevice, radius, radius);
            var colorData = new Color[radius * radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (var x = 0; x < radius; x++)
            {
                for (var y = 0; y < radius; y++)
                {
                    var index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public void Die(int damage)
        {
            if (_gameTime - _dieTime > _config.GameConfig.DamageTimeout)
            {
                _lives -= damage;
                if (_lives <= 0)
                {
                    Exit();
                }
                else
                {
                    _dieTime = _gameTime;
                }
            }
        }

        public void Eat(int food)
        {
            _score += food;
            if (_score % _config.GameConfig.FoodToLive == 0)
            {
                _lives += 1;
            }
            _snake = _snake.Increase(_config.SnakeConfig.CircleOffset);
        }
    }
}