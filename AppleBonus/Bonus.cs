using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using Eto.CustomControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.Bonuses;
using snake_game.Snake;

namespace AppleBonus
{
    public class Bonus : IBonus
    {
        readonly Config _config;
        readonly Random _random;
        bool _first = true;
        public readonly List<AppleBonus> Apples = new List<AppleBonus>();
        snake_game.MainGame.MainGame _game;

        public Bonus(Config cfg, Random rnd, snake_game.MainGame.MainGame game)
        {
            _config = cfg;
            _random = rnd;
            _game = game;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
        }

        public override void Update(GameTime gameTime, int fullTime, Dictionary<string, IBonus> plugins, CircleF[] snakePoints,
            Rectangle size)
        {
            if (_first)
            {
                for (var i = 0; i < _config.AppleCount; i++)
                {
                    Apples.Add(new AppleBonus(new Vector2(
                        _random.Next(size.X, size.X + size.Width),
                        _random.Next(size.Y, size.Y + size.Height)
                    ), new Vector2(
                        _random.NextDouble() > 0.5 ? 1 : -1,
                        _random.NextDouble() > 0.5 ? 1 : -1
                    ), _config));
                }
                _first = false;
            }

            var obstaclesL = new List<Segment>();
            if (plugins.ContainsKey("Brick"))
            {
                var brickManager = plugins["Brick"];
                foreach (var brick in brickManager.GetListProperty<Gettable>("Bricks"))
                {
                    var rect = brick.GetMethodResult<Rectangle>("GetRectangle");
                    obstaclesL.Add(new Segment(
                        new snake_game.Snake.Point(rect.X, rect.Y),
                        new snake_game.Snake.Point(rect.X + rect.Height, rect.Y)
                    ));
                    obstaclesL.Add(new Segment(
                        new snake_game.Snake.Point(rect.X, rect.Y),
                        new snake_game.Snake.Point(rect.X, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new snake_game.Snake.Point(rect.X, rect.Y + rect.Width),
                        new snake_game.Snake.Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new snake_game.Snake.Point(rect.X + rect.Height, rect.Y),
                        new snake_game.Snake.Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                }
            }
            var obstacles = obstaclesL.ToArray();

            var remove = new List<int>();
            for (var i = 0; i < Apples.Count; i++)
            {
                var apple = Apples[i];
                var appleCircle = apple.GetCircle();
                apple.Move(gameTime.ElapsedGameTime.TotalSeconds, fullTime, size, obstacles, snakePoints);
                if (appleCircle.Intersects(snakePoints.First()))
                {
                    _game.Eat(1);
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
                        _random.Next(size.Width - _config.Radius), _random.Next(size.Height - _config.Radius)
                    ), GetRandomDirection(), _config);
                } while (snakePoints.First().Intersects(newApple.GetCircle()));
                Apples.Add(newApple);
            }
        }

        Vector2 GetRandomDirection()
        {
            var degrees = _random.Next(0, 360);
            return new Vector2((float) Math.Cos(degrees), (float) Math.Sin(degrees));
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var apple in Apples)
            {
                sb.DrawCircle(apple.GetCircle(), _config.Sides, _config.AppleColor, _config.Thickness);
            }
        }

        public class AppleBonus : Gettable
        {
            Vector2 _position;
            Vector2 _direction;
            double _bounceTime;
            readonly Config _config;

            public AppleBonus(Vector2 pos, Vector2 direction, Config config)
            {
                _position = pos;
                _direction = direction;
                _config = config;
            }

            public CircleF GetCircle()
            {
                return new CircleF(_position, _config.Radius);
            }

            public void Move(double time, int fullTime, Rectangle size, Segment[] obstacles, CircleF[] snakePoints)
            {
                _position += new Vector2(
                                 _config.Speed * _direction.X,
                                 _config.Speed * _direction.Y
                             ) * (float) time;

                var MaxX = size.Width - _config.Radius;
                var MinX = _config.Radius;
                var MaxY = size.Height - _config.Radius;
                var MinY = _config.Radius;

                // Check for bounce.
                if (fullTime - _bounceTime > _config.BounceTimeout && (_position.X > MaxX || _position.X < MinX))
                {
                    var newDirection = MathUtils.Bounce(
                        new Line(1, 0, 0),
                        new snake_game.Snake.Point(_direction.X, _direction.Y)
                    );
                    _direction = new Vector2(newDirection.X, newDirection.Y);
                    _bounceTime = fullTime;
                }
                if (fullTime - _bounceTime > _config.BounceTimeout && (_position.Y > MaxY || _position.Y < MinY))
                {
                    var newDirection = MathUtils.Bounce(
                        new Line(0, 1, 0),
                        new snake_game.Snake.Point(_direction.X, _direction.Y)
                    );
                    _direction = new Vector2(newDirection.X, newDirection.Y);
                    _bounceTime = fullTime;
                }

                foreach (var o in obstacles)
                {
                    if (fullTime - _bounceTime > _config.BounceTimeout &&
                        MathUtils.Distance(o, new snake_game.Snake.Point(_position.X, _position.Y)) <= _config.Radius)
                    {
                        var newDirection = MathUtils.Bounce(
                            MathUtils.StandardLine(o.A, o.B),
                            new snake_game.Snake.Point(_direction.X, _direction.Y)
                        );
                        _direction = new Vector2(newDirection.X, newDirection.Y);
                        _bounceTime = fullTime;
                    }
                }

                var old = snakePoints[0];
                var circle = GetCircle();
                for (var i = 1; i < snakePoints.Length; i++)
                {
                    var current = snakePoints[i];
                    if (fullTime - _bounceTime > _config.BounceTimeout && current.Intersects(circle))
                    {
                        var seg = new Segment(
                            new snake_game.Snake.Point(old.Center.X, old.Center.Y),
                            new snake_game.Snake.Point(current.Center.X, current.Center.Y)
                        );
                        var newDirection = MathUtils.Bounce(
                            MathUtils.StandardLine(seg.A, seg.B),
                            new snake_game.Snake.Point(_direction.X, _direction.Y)
                        );
                        _direction = new Vector2(newDirection.X, newDirection.Y);
                        _bounceTime = fullTime;
                    }
                    old = current;
                }
            }

            public override TResult GetMethodResult<TResult>(string methodName)
            {
                switch (methodName)
                {
                    case nameof(GetCircle):
                        return (TResult) (object) GetCircle();
                    default:
                        return base.GetMethodResult<TResult>(methodName);
                }
            }
        }

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
    }
}