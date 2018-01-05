using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.MainGame;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace BrickBonus
{
    /// <inheritdoc />
    public class Bonus : BonusBase
    {
        private readonly Random _random;

        /// <summary>
        ///     Параметры бонуса
        /// </summary>
        public readonly Config Config;

        private Texture2D _texture;

        /// <inheritdoc />
        public Bonus(Config cfg, Random rnd)
        {
            Config = cfg;
            _random = rnd;
        }

        /// <summary>
        ///     Список всех кирпичей в игре
        /// </summary>
        public List<BrickBonus> Bricks { get; set; } = new List<BrickBonus>();

        /// <inheritdoc />
        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return !plugins.ContainsKey("Snake")
                ? new[] {"Snake"}
                : new string[] { };
        }

        /// <inheritdoc />
        public override void LoadContent(GraphicsDevice gd)
        {
            _texture = new Texture2D(gd, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        /// <inheritdoc />
        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles");

            if (fullTime % Config.ChanceTime == 0)
                if (_random.NextDouble() <= Config.NewChance)
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    BrickBonus newBrick;
                    do
                    {
                        newBrick = new BrickBonus(new Vector2(
                            _random.Next(size.Width), _random.Next(size.Height)
                        ), Config, fullTime);
                    } while (bigHead.Intersects((BoundingRectangle) newBrick.GetRectangle()));

                    Bricks.Add(newBrick);
                }

            foreach (var brick in Bricks)
                if ((fullTime - brick.SpawnTime) % Config.ChanceTime == 0)
                    brick.Move(
                        _random.NextDouble() > Config.MoveChance
                            ? Vector2.Zero
                            : new Vector2(
                                (float) (Config.Step / 2.0 - _random.NextDouble() * Config.Step),
                                (float) (Config.Step / 2.0 - _random.NextDouble() * Config.Step)
                            ),
                        new BagelWorld(size.Height, size.Width)
                    );

            foreach (var brick in Bricks)
            {
                var rect = brick.GetRectangle();
                if (snakePoints.First().Intersects((BoundingRectangle) rect))
                    plugins["Snake"].GetMethodResult<Void>("Damage");
            }

            return null;
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch sb)
        {
            foreach (var brick in Bricks) sb.Draw(_texture, brick.GetRectangle(), Config.BrickColor);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        /// <summary>
        ///     Класс для одного кирпича
        /// </summary>
        public class BrickBonus : Accessable
        {
            private readonly Config _config;

            /// <summary>
            ///     Игровое время, в которое эот кирпич появился
            /// </summary>
            public readonly int SpawnTime;

            private Vector2 _position; // Left upper corner

            /// <inheritdoc />
            public BrickBonus(Vector2 position, Config config, int spawnTime)
            {
                _position = position;
                _config = config;
                SpawnTime = spawnTime;
            }

            /// <summary>
            ///     Получает прямоугольник, который этот кирпич занимает
            /// </summary>
            /// <returns></returns>
            public Rectangle GetRectangle()
            {
                return new Rectangle(
                    (int) _position.X, (int) _position.Y, _config.Size, _config.Size
                );
            }

            /// <summary>
            ///     Передвигает кирпич на указанное расстояние
            /// </summary>
            /// <param name="move">Нас сколько его надо переместить</param>
            /// <param name="world">Тороидальный мир, в котором существует кирпич</param>
            public void Move(Vector2 move, BagelWorld world)
            {
                _position += move;
                var newPos = world.Normalize(new Point(_position.X, _position.Y));
                _position = new Vector2(newPos.X, newPos.Y);
            }

            /// <inheritdoc />
            public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
            {
                switch (methodName)
                {
                    case nameof(GetRectangle):
                        return (TResult) (object) GetRectangle();
                    default:
                        return base.GetMethodResult<TResult>(methodName, arguments);
                }
            }
        }
    }
}