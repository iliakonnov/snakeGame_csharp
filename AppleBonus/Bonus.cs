using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.MainGame;
using snake_game.Utils;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace AppleBonus
{
    /// <inheritdoc />
    public class Bonus : BonusBase
    {
        private readonly MainGame _game;

        private readonly Random _random;

        /// <summary>
        ///     Список яблок в игре
        /// </summary>
        public readonly List<AppleBonus> Apples = new List<AppleBonus>();

        /// <summary>
        ///     Настройки бонуса
        /// </summary>
        public readonly Config Config;

        private bool _first = true;

        /// <inheritdoc />
        public Bonus(Config cfg, Random rnd, MainGame game)
        {
            Config = cfg;
            _random = rnd;
            _game = game;
        }

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
        }

        /// <inheritdoc />
        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles").ToArray();

            if (_first)
            {
                for (var i = 0; i < Config.AppleCount; i++)
                    Apples.Add(new AppleBonus(new Vector2(
                        _random.Next(size.X, size.X + size.Width),
                        _random.Next(size.Y, size.Y + size.Height)
                    ), new Vector2(
                        _random.NextDouble() > 0.5 ? 1 : -1,
                        _random.NextDouble() > 0.5 ? 1 : -1
                    ), Config));

                _first = false;
            }

            var obstaclesL = new List<Segment>();
            if (plugins.ContainsKey("Brick"))
            {
                var brickManager = plugins["Brick"];
                foreach (var brick in brickManager.GetListProperty<Accessable>("Bricks"))
                {
                    var rect = brick.GetMethodResult<Rectangle>("GetRectangle");
                    obstaclesL.Add(new Segment(
                        new Point(rect.X, rect.Y),
                        new Point(rect.X + rect.Height, rect.Y)
                    ));
                    obstaclesL.Add(new Segment(
                        new Point(rect.X, rect.Y),
                        new Point(rect.X, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new Point(rect.X, rect.Y + rect.Width),
                        new Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new Point(rect.X + rect.Height, rect.Y),
                        new Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                }
            }

            var obstacles = obstaclesL.ToArray();

            var remove = new List<int>();
            for (var i = 0; i < Apples.Count; i++)
            {
                var apple = Apples[i];
                var appleCircle = apple.GetCircle();
                apple.Move(time.ElapsedGameTime.TotalSeconds, fullTime, size, obstacles, snakePoints);
                if (appleCircle.Intersects(snakePoints.First()))
                {
                    _game.Score(1);
                    plugins["Snake"].GetMethodResult<Void>("Increase", new object[] {1});
                    remove.Add(i);
                }
            }

            foreach (var index in remove)
            {
                Apples.RemoveAt(index);
                var bigHead = snakePoints.First();
                bigHead.Radius *= 2;
                AppleBonus newApple;
                do
                {
                    newApple = new AppleBonus(new Vector2(
                        _random.Next(size.Width - Config.Radius), _random.Next(size.Height - Config.Radius)
                    ), GetRandomDirection(), Config);
                } while (snakePoints.First().Intersects(newApple.GetCircle()));

                Apples.Add(newApple);
            }

            return null;
        }

        private Vector2 GetRandomDirection()
        {
            var degrees = _random.Next(0, 360);
            return new Vector2((float) Math.Cos(degrees), (float) Math.Sin(degrees));
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch sb)
        {
            foreach (var apple in Apples)
                sb.DrawCircle(apple.GetCircle(), Config.Sides, Config.AppleColor, Config.Thickness);
        }

        /// <inheritdoc />
        public override TResult GetProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Apples):
                    return (TResult) (object) Apples;
                default:
                    return base.GetProperty<TResult>(propertyName);
            }
        }


        /// <inheritdoc />
        /// <summary>
        ///     Класс для только одного яблока
        /// </summary>
        public class AppleBonus : Accessable
        {
            private readonly Config _config;
            private double _bounceTime;
            private Vector2 _direction;
            private Vector2 _position;

            /// <inheritdoc />
            public AppleBonus(Vector2 pos, Vector2 direction, Config config)
            {
                _position = pos;
                _direction = direction;
                _config = config;
            }

            /// <summary>
            ///     Возвращает окружность яблока
            /// </summary>
            /// <returns></returns>
            public CircleF GetCircle()
            {
                return new CircleF(_position, _config.Radius);
            }

            /// <summary>
            ///     Перемещает яблоко
            /// </summary>
            /// <param name="time">Прошедшее время с предыдущего перемещения</param>
            /// <param name="fullTime">Суммарное игровое время</param>
            /// <param name="size">Размер пигрового поля</param>
            /// <param name="obstacles">Препятствия, от которых яблоко отскакивает</param>
            /// <param name="snakePoints">Список точек змеи</param>
            public void Move(double time, int fullTime, Rectangle size, IEnumerable<Segment> obstacles,
                CircleF[] snakePoints)
            {
                _position += new Vector2(
                                 _config.Speed * _direction.X,
                                 _config.Speed * _direction.Y
                             ) * (float) time;

                var maxX = size.Width - _config.Radius;
                var minX = _config.Radius;
                var maxY = size.Height - _config.Radius;
                var minY = _config.Radius;

                // Check for bounce.
                if (fullTime - _bounceTime > _config.BounceTimeout && (_position.X > maxX || _position.X < minX))
                {
                    var newDirection = MathUtils.Bounce(
                        new Line(1, 0, 0),
                        new Point(_direction.X, _direction.Y)
                    );
                    _direction = new Vector2(newDirection.X, newDirection.Y);
                    _bounceTime = fullTime;
                }

                if (fullTime - _bounceTime > _config.BounceTimeout && (_position.Y > maxY || _position.Y < minY))
                {
                    var newDirection = MathUtils.Bounce(
                        new Line(0, 1, 0),
                        new Point(_direction.X, _direction.Y)
                    );
                    _direction = new Vector2(newDirection.X, newDirection.Y);
                    _bounceTime = fullTime;
                }

                foreach (var o in obstacles)
                    if (fullTime - _bounceTime > _config.BounceTimeout &&
                        MathUtils.Distance(o, new Point(_position.X, _position.Y)) <= _config.Radius)
                    {
                        var newDirection = MathUtils.Bounce(
                            MathUtils.StandardLine(o.A, o.B),
                            new Point(_direction.X, _direction.Y)
                        );
                        _direction = new Vector2(newDirection.X, newDirection.Y);
                        _bounceTime = fullTime;
                    }

                var old = snakePoints[0];
                var circle = GetCircle();
                for (var i = 1; i < snakePoints.Length; i++)
                {
                    var current = snakePoints[i];
                    if (fullTime - _bounceTime > _config.BounceTimeout && current.Intersects(circle))
                    {
                        var seg = new Segment(
                            new Point(old.Center.X, old.Center.Y),
                            new Point(current.Center.X, current.Center.Y)
                        );
                        var newDirection = MathUtils.Bounce(
                            MathUtils.StandardLine(seg.A, seg.B),
                            new Point(_direction.X, _direction.Y)
                        );
                        _direction = new Vector2(newDirection.X, newDirection.Y);
                        _bounceTime = fullTime;
                    }

                    old = current;
                }
            }

            /// <inheritdoc />
            public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
            {
                switch (methodName)
                {
                    case nameof(GetCircle):
                        return (TResult) (object) GetCircle();
                    default:
                        return base.GetMethodResult<TResult>(methodName, arguments);
                }
            }
        }
    }
}