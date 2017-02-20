using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using snake_game.Snake;

namespace snake_game.MainGame
{
    public partial class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        SnakeModel _snake;
        SnakeModel _newSnake;
        BagelWorld _world;
        Controller _ctrl;
        Color[] _colors;
        Fog _fog;
        readonly Debug _dbg;
        readonly int SnakeCircleSize = 40;
        readonly int SnakeCircleOffset = 5;
        readonly int SnakeInitLen = 30;
        readonly Config _config;

        public MainGame(Config config)
        {
            _graphics = new GraphicsDeviceManager(this);

            IsMouseVisible = config.ScreenConfig.IsMouseVisible;
            _graphics.IsFullScreen = config.ScreenConfig.IsFullScreen;
            _graphics.PreferredBackBufferHeight = config.ScreenConfig.ScreenHeight;
            _graphics.PreferredBackBufferWidth = config.ScreenConfig.ScreenWidth;

            SnakeCircleOffset = config.SnakeConfig.CircleOffset;
            SnakeCircleSize = config.SnakeConfig.CircleSize;
            SnakeInitLen = config.SnakeConfig.InitLen;

            _config = config;

            Content.RootDirectory = "Content";

            _dbg = new Debug(this, _config.GameConfig.DebugColor);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fog = new Fog(GraphicsDevice, _config.GameConfig.FogColor.Item1, _config.GameConfig.FogColor.Item2);
			_snake = new SnakeModel(new Snake.Point(400, 150), 0).Increase(SnakeInitLen*SnakeCircleOffset);
            _ctrl = new Controller(30);

            if (_config.SnakeConfig.Colors == null)
            {
                var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

                var colors = new List<Color>();
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color))
                    {
                        colors.Add((Color) propertyInfo.GetValue(null, null));
                    }
                }
                _colors = colors.ToArray();
            }
            else
            {
                _colors = _config.SnakeConfig.Colors;
            }

            _dbg.LoadContent();
            _dbg.IsEnabled = _config.GameConfig.DebugShow;

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _dbg.Update(gameTime);
            var control = _ctrl.Control(Keyboard.GetState());

            if (control.Debug)
            {
                _dbg.IsEnabled = !_dbg.IsEnabled;
            }
            if (control.IsExit)
            {
                Exit();
            }
            if (control.Turn.ToTurn)
            {
                if (control.Turn.ReplaceTurn)
                {
                    _snake = _snake.TurnAt(control.Turn.TurnDegrees);
                }
                else
                {
					_snake = _snake.Turn(control.Turn.TurnDegrees);
                }
            }

            _newSnake = _snake.ContinueMove(150 * gameTime.ElapsedGameTime.Milliseconds/1000);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var circle = CreateCircleTexture(SnakeCircleSize);
            var newSize = _dbg.Size();
            _world = new BagelWorld(newSize.Height - SnakeCircleSize, newSize.Width - SnakeCircleSize);
            var snakePoints = _newSnake.GetSnakeAsPoints(SnakeCircleOffset).Select(x =>  _world.Normalize(x)).ToArray();

            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _dbg.Draw(snakePoints);

            for (var i = 0; i < snakePoints.Length; i++)
            {
                _spriteBatch.Draw(
                    circle,
                    new Vector2(snakePoints[i].X, snakePoints[i].Y),
                    _colors[i % _colors.Length]
                );
            }
            _snake = _newSnake;

            if (_config.GameConfig.FogEnabled) _fog.CreateFog(_spriteBatch, newSize, SnakeCircleSize);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        Texture2D CreateCircleTexture(int radius)
        {
            var texture = new Texture2D(GraphicsDevice, radius, radius);
            var colorData = new Color[radius*radius];

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

    }
}