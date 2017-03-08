using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace snake_game.MainGame
{

	public partial class MainGame
	{
		class Debug
		{
			readonly MainGame _game;
			SpriteFont _font;
			Texture2D _dummyTexture;
			Color _color;
			int _frames;
			int _fps;
			int _time;
			const int Width = 300;
			public bool IsEnabled;

			public Debug(MainGame game, Color color)
			{
				_game = game;
				_color = color;
			}

			public Rectangle Size()
			{
				return new Rectangle(0, 0,
					IsEnabled ?
						_game.Window.ClientBounds.Width - Width :
						_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height
				);
			}

			public void LoadContent()
			{
				_font = _game.Content.Load<SpriteFont>("DejaVu Sans Mono");
				_dummyTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
				_dummyTexture.SetData(new[] { Color.White });
			}

			public void Update(GameTime gameTime)
			{
				if (IsEnabled)
				{
					_time += gameTime.ElapsedGameTime.Milliseconds;
					if (_time >= 1000)
					{
						_fps = _frames / (_time / 1000);
						_time = 0;
						_frames = 0;
					}
				}
			}

			public void Draw(Snake.Point[] snakePoints)
			{
				if (IsEnabled)
				{
					_frames++;
					_game._spriteBatch.Draw(_dummyTexture, new Rectangle(
						_game.Window.ClientBounds.Width - Width,
						0,
						_game.Window.ClientBounds.Width,
						_game.Window.ClientBounds.Height),
						_color
					);

					var debugString =
						"    DEBUG\n" +
						$"Game Size: ({Size().Width}; {Size().Height})\n" +
						$"World Size: ({_game._world.Width}; {_game._world.Height})\n" +
						$"FPS: {_fps}\n" +
						$"Snake length: {_game._snake.Length} pixel(s)\n" +
						$"Snake length: {snakePoints.Length} circle(s)\n" +
						$"Snake direction: {_game._snake.Direction}\n" +
						$"Head point: ({snakePoints.Last().X}; {snakePoints.Last().Y})";

					_game._spriteBatch.DrawString(_font, debugString, new Vector2(
						_game.Window.ClientBounds.Width - Width, 0
					), Color.Black);
				}
			}
		}
	}
}