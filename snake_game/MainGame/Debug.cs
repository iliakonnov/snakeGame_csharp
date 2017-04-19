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

            public Rectangle Size(GameWindow window)
            {
                return new Rectangle(0, 0,
                    IsEnabled
                        ? window.ClientBounds.Width - Width
                        : window.ClientBounds.Width, window.ClientBounds.Height
                );
            }

            public void LoadContent(GraphicsDevice gd)
            {
                _dummyTexture = new Texture2D(gd, 1, 1);
                _dummyTexture.SetData(new[] {Color.White});
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

            public void Draw(Snake.Point[] snakePoints, SpriteBatch spriteBatch, GameWindow window)
            {
                if (IsEnabled)
                {
                    _frames++;
                    spriteBatch.Draw(_dummyTexture, new Rectangle(
                            window.ClientBounds.Width - Width,
                            0,
                            window.ClientBounds.Width,
                            window.ClientBounds.Height),
                        _color
                    );

                    var size = Size(window);
                    var debugString =
                        "    DEBUG\n" +
                        $"Game Size: ({size.Width}; {size.Height})\n" +
                        $"World Size: ({_game._world.Width}; {_game._world.Height})\n" +
                        $"FPS: {_fps}\n" +
                        $"Game time: {_game._gameTime}\n" +
                        $"Damage timout: {_game._gameTime - _game._dieTime}\n" +
                        $"Snake length: {_game._snake.Length} pixel(s)\n" +
                        $"Snake length: {snakePoints.Length} circle(s)\n" +
                        $"Snake direction: {_game._snake.Direction}\n" +
                        $"Head point: ({snakePoints.First().X}; {snakePoints.First().Y})";

                    spriteBatch.DrawString(_game._font, debugString, new Vector2(
                        window.ClientBounds.Width - Width, 0
                    ), Color.Black);
                }
            }
        }
    }
}