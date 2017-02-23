using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using snake_game.Snake;
using System;

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

			Content.RootDirectory = "Content";

			_dbg = new Debug(this, _config.GameConfig.DebugColor);
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_fog = new Fog(GraphicsDevice, _config.GameConfig.FogColor.Item1, _config.GameConfig.FogColor.Item2);
			_snake = new SnakeModel(new Snake.Point(400, 150), 0).Increase(_config.SnakeConfig.InitLen * _config.SnakeConfig.CircleOffset);
			_ctrl = new Controller(30);

			if (_config.SnakeConfig.Colors == null)
			{
				var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

				var colors = new List<Color>();
				foreach (var propertyInfo in properties)
				{
					if (propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color))
					{
						var col = (Color)propertyInfo.GetValue(null, null);
						if (col != _config.GameConfig.BackgroundColor && col.A == 255)
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

			_newSnake = _snake.ContinueMove(_config.SnakeConfig.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

			var points = _newSnake.GetSnakeAsPoints(_config.SnakeConfig.CircleOffset);
			var headCenter = points.First();

			var halfSize = _config.SnakeConfig.CircleSize / 2;
			var head = new CircleF(
				new Vector2(headCenter.X, headCenter.Y), halfSize
			);
			for (int i = _intersectStart; i < points.Length; i++)
			{
				var current = new CircleF(
					new Vector2(points[i].X, points[i].Y), halfSize
				);
				if (head.Intersects(current))
				{
					Exit();
				}
			}
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			var halfSize = _config.SnakeConfig.CircleSize / 2;
			var circle = CreateCircleTexture(_config.SnakeConfig.CircleSize);
			var newSize = _dbg.Size();
			_world = new BagelWorld(newSize.Height, newSize.Width);
			var snakePoints = _newSnake.GetSnakeAsPoints(_config.SnakeConfig.CircleOffset).Select(x => _world.Normalize(x)).ToArray();

			_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			_dbg.Draw(snakePoints);

			for (var i = 0; i < snakePoints.Length; i++)
			{
				_spriteBatch.Draw(
					circle,
					new Vector2(
						snakePoints[i].X - halfSize,
						snakePoints[i].Y - halfSize
					),
					_colors[i % _colors.Length]
				);
			}
			_snake = _newSnake;

			if (_config.GameConfig.FogEnabled) _fog.CreateFog(_spriteBatch, newSize, (int)Math.Round(_config.SnakeConfig.CircleSize * _config.GameConfig.FogSizeMultiplier));

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

	}
}