using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.Bonuses;
using snake_game.MainGame;
using snake_game.Snake;

namespace BrickBonus
{
    public class Bonus : BonusBase
    {
        readonly Random _random;
        readonly Config _config;
        MainGame _game;
        Texture2D _texture;


        public List<BrickBonus> Bricks { get; set; } = new List<BrickBonus>();

        public Bonus(Config cfg, Random rnd, MainGame game)
        {
            _config = cfg;
            _random = rnd;
            _game = game;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public override IReadOnlyDictionary<string, Gettable> Update(GameTime gameTime, int fullTime,
            IReadOnlyDictionary<string, BonusBase> plugins, CircleF[] snakePoints,
            Rectangle size, IReadOnlyDictionary<string, IReadOnlyDictionary<string, Gettable>> events)
        {
            if (fullTime % _config.ChanceTime == 0)
            {
                foreach (var brick in Bricks)
                {
                    brick.Move(
                        _random.NextDouble() > _config.MoveChance
                            ? new Vector2()
                            : new Vector2(
                                (float) (_config.Step / 2.0 - _random.NextDouble() * _config.Step),
                                (float) (_config.Step / 2.0 - _random.NextDouble() * _config.Step)
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

            return null;
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var brick in Bricks)
            {
                sb.Draw(_texture, brick.GetRectangle(), _config.BrickColor);
            }
        }

        public class BrickBonus : Gettable
        {
            Vector2 _position; // Left upper corner
            Config _config;

            public BrickBonus(Vector2 position, Config config)
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
                var newPos = world.Normalize(new snake_game.Snake.Point(_position.X, _position.Y));
                _position = new Vector2(newPos.X, newPos.Y);
            }

            public override TResult GetMethodResult<TResult>(string methodName)
            {
                switch (methodName)
                {
                    case nameof(GetRectangle):
                        return (TResult) (object) GetRectangle();
                    default:
                        return base.GetMethodResult<TResult>(methodName);
                }
            }
        }

        public override List<TResult> GetListProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Bricks):
                    return Bricks.Cast<TResult>().ToList();
                default:
                    return base.GetListProperty<TResult>(propertyName);
            }
        }

        public override void SetListProperty(string propertyName, IEnumerable<object> newValue)
        {
            switch (propertyName)
            {
                case nameof(Bricks):
                    Bricks = newValue.Cast<BrickBonus>().ToList();
                    break;
                default:
                    base.SetListProperty(propertyName, newValue);
                    break;
            }
        }
    }
}