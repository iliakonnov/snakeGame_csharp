using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using snake_game.Snake;

namespace snake_game.MainGame
{
    public class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        SnakeModel _snake;
        SnakeModel _newSnake;
        BagelWorld _world;
        Controller _ctrl;
        Color[] _colors;
        Fog _fog;
        readonly Debug _dbg = new Debug();
        const int SnakeCircleSize = 40;
        const int SnakeCircleOffset = 5;
        const int SnakeInitLen = 30;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fog = new Fog(GraphicsDevice, Color.DarkSlateBlue, Color.Transparent);
			_snake = new SnakeModel(new Snake.Point(400, 150), 0).Increase(SnakeInitLen*SnakeCircleOffset);
            _ctrl = new Controller(30);

            var properties = typeof(Color).GetProperties(BindingFlags.Public|BindingFlags.Static);

            var colors = new List<Color>();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color))
                {
                    colors.Add((Color) propertyInfo.GetValue(null, null));
                }
            }
            _colors = colors.ToArray();

            _dbg.LoadContent(Content, GraphicsDevice);

            #if DEBUG
            _dbg.Enable();
            #endif

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
            var newSize = _dbg.Size(Window.ClientBounds.Width, Window.ClientBounds.Height);
            _world = new BagelWorld(newSize.Height - SnakeCircleSize, newSize.Width - SnakeCircleSize);
            var snakePoints = _newSnake.GetSnakeAsPoints(SnakeCircleOffset).Select(x =>  _world.Normalize(x)).ToArray();

            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _dbg.Draw(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height,
                snakePoints, _snake, _world);

            for (var i = 0; i < snakePoints.Length; i++)
            {
                _spriteBatch.Draw(
                    circle,
                    new Vector2(snakePoints[i].X, snakePoints[i].Y),
                    _colors[i % _colors.Length]
                );
            }
            _snake = _newSnake;

            _fog.CreateFog(_spriteBatch, newSize, (int)(SnakeCircleSize*1.5));

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        Texture2D CreateCircleTexture(int radius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius*radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
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