using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using snake_game.Snake;


namespace snake_game.MainGame
{
    public class Debug
    {
        SpriteFont _font;
        Texture2D _dummyTexture;
        int _frames;
        int _fps;
        int _time;
        const int Width = 300;
        public bool IsEnabled;

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public Rectangle Size(int width, int height)
        {
            return new Rectangle(0, 0, IsEnabled ? width - Width : width, height);
        }

        public void LoadContent(ContentManager Content, GraphicsDevice gd)
        {
            _font = Content.Load<SpriteFont>("DejaVu Sans Mono");
            _dummyTexture = new Texture2D(gd, 1, 1);
            _dummyTexture.SetData(new [] { Color.White });
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

        public void Draw(SpriteBatch sb, int width, int height,
            Snake.Point[] snakePoints, SnakeModel snake, BagelWorld world)
        {
            if (IsEnabled)
            {
                _frames++;
                sb.Draw(_dummyTexture, new Rectangle(width - Width, 0, Width, height), Color.LightGray);

                var debugString =
                    "    DEBUG\n" +
                    $"Game size: ({width-Width}; {height})\n" +
                    $"World size: ({world.Width}; {world.Height})\n" +
                    $"FPS: {_fps}\n" +
                    $"Snake length: {snakePoints.Length}\n" +
                    $"Snake direction: {snake.Direction}\n" +
                    $"Head point: ({snakePoints.Last().X}; {snakePoints.Last().Y})";

                sb.DrawString(_font, debugString, new Vector2(width - Width, 0), Color.Black);
            }
        }
    }
}