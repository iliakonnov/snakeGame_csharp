﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
    public class AppleManager : IBonusManager
    {
        public string Name => "Apple";
        readonly Config.BonusConfigClass.AppleConfigClass _config;
        readonly Random _random;
        bool _first = true;
        readonly List<AppleBonus> _apples = new List<AppleBonus>();
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
                    )));
                }
                _first = false;
            }

            var remove = new List<int>();
            for (var i = 0; i < _apples.Count; i++)
            {
                var apple = _apples[i];
                apple.Move(gameTime.ElapsedGameTime.TotalSeconds, _config.Speed, size, _config.Radius);
                if (apple.GetCircle(_config.Radius).Intersects(snakePoints.First()))
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
                Vector2 direction = new Vector2(
                    _random.NextDouble() > 0.5 ? 1 : -1,
                    _random.NextDouble() > 0.5 ? 1 : -1
                );
                do
                {
                    newApple = new AppleBonus(new Vector2(
                        _random.Next(size.Width), _random.Next(size.Height)
                    ), direction);
                } while (snakePoints.First().Intersects(newApple.GetCircle(_config.Radius)));
                _apples.Add(newApple);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var apple in _apples)
            {
                sb.DrawCircle(apple.GetCircle(_config.Radius), _config.Sides, _config.AppleColor, _config.Thickness);
            }
        }
    }

    class AppleBonus
    {
        public Vector2 position;
        Vector2 _direction;

        public AppleBonus(Vector2 pos, Vector2 direction)
        {
            position = pos;
            _direction = direction;
        }

        public CircleF GetCircle(int radius)
        {
            return new CircleF(position, radius);
        }

        public void Move(double time, double speed, Rectangle size, int radius)
        {
            position += new Vector2(
                            (float)(speed * _direction.X),
                            (float)(speed * _direction.Y)
                        ) * (float)time;
            
            var MaxX = size.Width - radius;
            var MinX = 0;
            var MaxY = size.Height - radius;
            var MinY = 0;

            // Check for bounce.
            if (position.X > MaxX)
            {
                _direction.X *= -1;
                position.X = MaxX;
            }

            else if (position.X < MinX)
            {
                _direction.X *= -1;
                position.X = MinX;
            }

            if (position.Y > MaxY)
            {
                _direction.Y *= -1;
                position.Y = MaxY;
            }

            else if (position.Y < MinY)
            {
                _direction.Y *= -1;
                position.Y = MinY;
            }
        }
    }
}