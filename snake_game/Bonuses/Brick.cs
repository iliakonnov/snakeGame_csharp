using snake_game.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using snake_game.Snake;

namespace snake_game.Bonuses
{
    public class BrickManager : IBonusManager
    {
        readonly Random _random;
        readonly Config.BonusConfigClass.BrickConfigClass _config;
        MainGame.MainGame _game;
        Texture2D _texture;
        public List<BrickBonus> Bricks = new List<BrickBonus>();

        public string Name => "brick";

        public BrickManager(Config.BonusConfigClass.BrickConfigClass cfg, Random rnd, MainGame.MainGame game)
        {
            _config = cfg;
            _random = rnd;
            _game = game;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints,
            Rectangle size)
        {
            if (fullTime % _config.ChanceTime == 0)
            {
                foreach (var brick in Bricks)
                {
                    brick.Move(
                        _random.NextDouble() > _config.MoveChance
                            ? new Vector2()
                            : new Vector2(
                                (float) (_config.Step / 2 - _random.NextDouble() * _config.Step),
                                (float) (_config.Step / 2 - _random.NextDouble() * _config.Step)
                            ),
                        new BagelWorld(size.Height, size.Width)
                    );
                }
                if (_random.NextDouble() <= _config.NewChance)
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    BrickBonus newBrick;
                    do
                    {
                        newBrick = new BrickBonus(new Vector2(
                            _random.Next(size.Width), _random.Next(size.Height)
                        ), _config);
                    } while (bigHead.Intersects(newBrick.GetRectangle()));
                    Bricks.Add(newBrick);
                }
            }

            foreach (var brick in Bricks)
            {
                var rect = brick.GetRectangle();
                rect.X += rect.Width / 2;
                rect.Y += rect.Height / 2;
                if (snakePoints.First().Intersects(rect))
                {
                    _game.Die(1);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var brick in Bricks)
            {
                sb.Draw(_texture, brick.GetRectangle(), _config.BrickColor);
            }
        }

        public class BrickBonus
        {
            Vector2 _position; // Left upper corner
            Config.BonusConfigClass.BrickConfigClass _config;

            public BrickBonus(Vector2 position, Config.BonusConfigClass.BrickConfigClass config)
            {
                _position = position;
                _config = config;
            }

            public Rectangle GetRectangle()
            {
                return new Rectangle(
                    (int) _position.X, (int) _position.Y, _config.Size, _config.Size
                );
            }

            public void Move(Vector2 move, BagelWorld world)
            {
                _position += move;
                var newPos = world.Normalize(new Snake.Point(_position.X, _position.Y));
                _position = new Vector2(newPos.X, newPos.Y);
            }
        }
    }
}