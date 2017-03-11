﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;
using snake_game.Snake;

namespace snake_game.Bonuses
{
    public class AppleManager : IBonusManager
    {
        public string Name => "Apple";
        readonly Config.BonusConfigClass.AppleConfigClass _config;
        readonly Random _random;
        bool _first = true;
        public readonly List<AppleBonus> _apples = new List<AppleBonus>();
        MainGame.MainGame _game;

        public AppleManager(Config.BonusConfigClass.AppleConfigClass cfg, Random rnd, MainGame.MainGame game)
        {
            _config = cfg;
            _random = rnd;
            _game = game;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
        }

        public void Update(GameTime gameTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
        {
            if (_first)
            {
                for (var i = 0; i < _config.AppleCount; i++)
                {
                    _apples.Add(new AppleBonus(new Vector2(
                        _random.Next(size.X, size.X + size.Width),
                        _random.Next(size.Y, size.Y + size.Height)
                    ), new Vector2(
                        _random.NextDouble() > 0.5 ? 1 : -1,
                        _random.NextDouble() > 0.5 ? 1 : -1
                    ), _config));
                }
                _first = false;
            }

            var brickManagers = bonuses.Where(x => x.Name == "brick").ToArray();
            var obstaclesL = new List<Segment>();
            if (brickManagers.Length != 0)
            {
                var brickManager = (BrickManager) brickManagers[0];
                foreach (var brick in brickManager.Bricks)
                {
                    var rect = brick.GetRectangle();
                    obstaclesL.Add(new Segment(
                        new Snake.Point(rect.X, rect.Y),
                        new Snake.Point(rect.X + rect.Height, rect.Y)
                    ));
                    obstaclesL.Add(new Segment(
                        new Snake.Point(rect.X, rect.Y),
                        new Snake.Point(rect.X, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new Snake.Point(rect.X, rect.Y + rect.Width),
                        new Snake.Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                    obstaclesL.Add(new Segment(
                        new Snake.Point(rect.X + rect.Height, rect.Y),
                        new Snake.Point(rect.X + rect.Height, rect.Y + rect.Width)
                    ));
                }
            }
            var obstacles = obstaclesL.ToArray();

            var remove = new List<int>();
            for (var i = 0; i < _apples.Count; i++)
            {
                var apple = _apples[i];
                var appleCircle = apple.GetCircle();
                apple.Move(gameTime.ElapsedGameTime.TotalSeconds, size, obstacles, snakePoints);
                if (appleCircle.Intersects(snakePoints.First()))
                {
                    _game.Eat(1);
                    remove.Add(i);
                }
            }
            foreach (var index in remove)
            {
                _apples.RemoveAt(index);
                var bigHead = snakePoints.First();
                bigHead.Radius *= 2;
                AppleBonus newApple;
                do
                {
                    newApple = new AppleBonus(new Vector2(
                        _random.Next(size.Width - _config.Radius), _random.Next(size.Height - _config.Radius)
                    ), GetRandomDirection(), _config);
                } while (snakePoints.First().Intersects(newApple.GetCircle()));
                _apples.Add(newApple);
            }
        }

        Vector2 GetRandomDirection()
        {
            var degrees = _random.Next(0, 360);
            return new Vector2((float) Math.Cos(degrees), (float) Math.Sin(degrees));
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var apple in _apples)
            {
                sb.DrawCircle(apple.GetCircle(), _config.Sides, _config.AppleColor, _config.Thickness);
            }
        }

        public class AppleBonus
        {
            Vector2 _position;
            Vector2 _direction;
            readonly Config.BonusConfigClass.AppleConfigClass _config;

            public AppleBonus(Vector2 pos, Vector2 direction, Config.BonusConfigClass.AppleConfigClass config)
            {
                _position = pos;
                _direction = direction;
                _config = config;
            }

            public CircleF GetCircle()
            {
                return new CircleF(_position, _config.Radius);
            }

            public void Move(double time, Rectangle size, Segment[] obstacles, CircleF[] snakePoints)
            {
                _position += new Vector2(
                                _config.Speed * _direction.X,
                                _config.Speed * _direction.Y
                            ) * (float)time;

                var MaxX = size.Width - _config.Radius;
                var MinX = _config.Radius;
                var MaxY = size.Height - _config.Radius;
                var MinY = _config.Radius;

                // Check for bounce.
                if (_position.X > MaxX || _position.X < MinX) bounce(new Line(1, 0, 0));
                if (_position.Y > MaxY || _position.Y < MinY) bounce(new Line(0, 1, 0));

                foreach (var o in obstacles)
                {
                    if (MathUtils.Distance(o, new Snake.Point(_position.X, _position.Y)) <= _config.Radius)
                    {
                        bounce(MathUtils.StandardLine(o.A, o.B));
                    }
                }

                var old = snakePoints[0];
                var circle = GetCircle();
                for (var i = 1; i < snakePoints.Length; i++)
                {
                    var current = snakePoints[i];
                    if (current.Intersects(circle))
                    {
                        var seg = new Segment(
                            new Snake.Point(old.Center.X, old.Center.Y),
                            new Snake.Point(current.Center.X, current.Center.Y)
                        );
                        bounce(MathUtils.StandardLine(seg.A, seg.B));
                    }
                    old = current;
                }
            }

            void bounce(Line line)
            {
                // a, b, c are not Line parameters, but triangle
                var a = line.A;
                var b = line.B;
                var c = (float)Math.Sqrt(a * a + b * b);
                a /= c;
                b /= c;
                var a2 = a * a;
                var b2 = b * b;
                var doubleAB = 2 * a * b;
                _direction = new Vector2(
                    (b2 - a2) * _direction.X +  // (b^2 - a^2)Vx +
                    doubleAB * _direction.Y,  // + 2ab * Vy;
                    doubleAB * _direction.X +  //  2ab * Vx +
                    (a2 - b2) * _direction.Y  // + (a^2 - b^2)Vy
                );
            }
        }
    }
}