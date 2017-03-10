using System;
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
                apple.Move(gameTime.ElapsedGameTime.TotalSeconds, size, obstacles);
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
            var x = _random.NextDouble();
            var y = _random.NextDouble();
            if (x <= 0.5)
            {
                x -= 1;
            }
            if (y <= 0.5)
            {
                y -= 1;
            }
            return new Vector2((float) x, (float) y);
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
            Vector2 position;
            Vector2 _direction;
            readonly Config.BonusConfigClass.AppleConfigClass _config;

            public AppleBonus(Vector2 pos, Vector2 direction, Config.BonusConfigClass.AppleConfigClass config)
            {
                position = pos;
                _direction = direction;
                _config = config;
            }

            public CircleF GetCircle()
            {
                return new CircleF(position, _config.Radius);
            }

            public void Move(double time, Rectangle size, Segment[] obstacles)
            {
                position += new Vector2(
                                _config.Speed * _direction.X,
                                _config.Speed * _direction.Y
                            ) * (float)time;

                var MaxX = size.Width - _config.Radius;
                var MinX = _config.Radius;
                var MaxY = size.Height - _config.Radius;
                var MinY = _config.Radius;

                // Check for bounce.
                if (position.X > MaxX || position.X < MinX) bounce(1, 0);
                if (position.Y > MaxY || position.Y < MinY) bounce(0, 1);

                foreach (var o in obstacles)
                {

                }
            }

            void bounce(double a, double b)
            {
                var c = a * a + b * b;
                a /= c;
                b /= c;
                var a2 = a * a;
                var b2 = b * b;
                var doubleAB = 2 * a * b;
                _direction = new Vector2(
                    (float) (
                        (b2 - a2) * _direction.X +  //  (b^2 - a^2)Vx +
                        doubleAB * _direction.Y),  // + 2ab * Vy
                    (float) (
                        doubleAB * _direction.X +  //   2ab * Vx +
                        (a2 - b2) * _direction.Y)  // + (a^2 - b^2)Vy
                );
            }
        }
    }
}