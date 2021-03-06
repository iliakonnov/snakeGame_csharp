﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.MainGame;
using snake_game.Snake;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace Snake
{
    public class Bonus : BonusBase
    {
        public SnakeModel Snake;
        public int DamagedTime;
        public Config Config;
        
        private Color[] _colors;
        private int _intersectStart;
        private MainGame _game;
        private Texture2D _circle;
        private int _gameTime;
        private IController _ctrl;
        private Point[] _snakePoints;
        private int _damage;

        public bool Invulnerable => _gameTime - DamagedTime < Config.DamageTimeout;
        public CircleF[] SnakeCircles = { };

        public Bonus(Config cfg, MainGame game)
        {
            Config = cfg;
            _game = game;
        }

        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return new string[] { };
        }

        public override void LoadContent(GraphicsDevice gd)
        {
            Snake =
                new SnakeModel(new Point(400, 150), 0).Increase(
                    Config.InitLen * Config.CircleOffset);

            if (Config.Colors == null)
            {
                var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

                _colors = (from propertyInfo in properties
                    where propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color)
                    select (Color) propertyInfo.GetValue(null, null)
                    into col
                    where col != Config.HeadColor && col.A == 255
                    select col).ToArray();
            }
            else
            {
                _colors = Config.Colors;
            }

            DamagedTime = -Config.DamageTimeout;
            _circle = CreateCircleTexture(Config.CircleSize, gd);

            _snakePoints = Snake.GetSnakeAsPoints(Config.CircleOffset);
            var headCenter = _snakePoints.First();
            var head = new CircleF(
                new Vector2(headCenter.X, headCenter.Y), Config.CircleSize
            );

            var halfSize = Config.CircleSize / 2;
            SnakeCircles = _snakePoints.Select(p => new CircleF(new Vector2(p.X, p.Y), halfSize)).ToArray();

            var i = 1;
            bool intersects;
            do
            {
                var current = new CircleF(
                    new Vector2(_snakePoints[i].X, _snakePoints[i].Y), Config.CircleSize
                );
                i++;
                intersects = head.Intersects(current);
            } while (intersects);
            _intersectStart = i;

            switch (Config.ControlType)
            {
                case "traditional":
                    _ctrl = new ControllerTraditional();
                    break;
                case "small":
                    _ctrl = new ControllerSmall(Config.TurnSize ?? 30);
                    break;
                default:
                    throw new ArgumentException("Unknown control type");
            }
        }

        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard, IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size, IReadOnlyDictionary<string, Accessable> events)
        {
            var control = _ctrl.Control(keyboard);
            if (control.Turn.ToTurn)
            {
                Snake = control.Turn.ReplaceTurn
                    ? Snake.TurnAt(control.Turn.TurnDegrees)
                    : Snake.Turn(control.Turn.TurnDegrees);
            }

            _gameTime = fullTime;

            Snake = Snake.ContinueMove(Config.Speed * time.ElapsedGameTime.Milliseconds / 1000);

            var halfSize = Config.CircleSize / 2;
            var points = Snake.GetSnakeAsPoints(Config.CircleOffset);
            SnakeCircles = new CircleF[points.Length];
            _snakePoints = new Point[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                var normPoint = _game.World.Normalize(points[i]);
                SnakeCircles[i] = new CircleF(new Vector2(normPoint.X, normPoint.Y), halfSize);
                _snakePoints[i] = normPoint;
            }
            var head = SnakeCircles.First();

            for (var i = _intersectStart; i < SnakeCircles.Length; i++)
            {
                if (head.Intersects(SnakeCircles[i]))
                {
                    Damage();
                }
            }

            var snakeEvents = new SnakeEvents();
            if (events.ContainsKey("Snake"))
            {
                var oldEvents = (SnakeEvents) events["Snake"];
                if (_damage != 0) // Был нанесен ещё урон
                {
                    snakeEvents.DamageInfo.Damage = _damage;
                    snakeEvents.DamageInfo.FirstTime = true;
                }
                else if (oldEvents.DamageInfo.FirstTime) // Один тик назад был нанесен урон
                {
                    snakeEvents.DamageInfo.Damage = oldEvents.DamageInfo.Damage;
                    snakeEvents.DamageInfo.FirstTime = false; // Помечает, что первый тик пройден
                }
                else if (!oldEvents.DamageInfo.FirstTime && // Два тика назад был нанесен урон 
                         oldEvents.DamageInfo.Damage != 0 && // Урон всё-таки был
                         !oldEvents.DamageInfo.Prevented) // И его никто не отменил
                {
                    _game.Damage(oldEvents.DamageInfo.Damage);
                    snakeEvents.DamageInfo.Damage = 0;
                    snakeEvents.DamageInfo.FirstTime = false;
                }
            }
            _damage = 0;

            return snakeEvents;
        }

        public override void Draw(SpriteBatch sb)
        {
            var halfSize = Config.CircleSize / 2;

            for (var i = _snakePoints.Length - 1; i >= 0; i--)
            {
                sb.Draw(
                    _circle,
                    new Vector2(
                        _snakePoints[i].X - halfSize,
                        _snakePoints[i].Y - halfSize
                    ),
                    Invulnerable
                        ? Config.DamageColor
                        : i == 0
                            ? Config.HeadColor ?? _colors[i % _colors.Length]
                            : _colors[i % _colors.Length]
                );
            }
        }

        private Texture2D CreateCircleTexture(int radius, GraphicsDevice gd)
        {
            var texture = new Texture2D(gd, radius, radius);
            var colorData = new Color[radius * radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (var x = 0; x < radius; x++)
            {
                for (var y = 0; y < radius; y++)
                {
                    var index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        private class SnakeEvents : Accessable
        {
            public class Damaged : Accessable
            {
                public bool FirstTime = true;
                public int Damage;
                public bool Prevented;

                public override TResult GetProperty<TResult>(string propertyName)
                {
                    switch (propertyName)
                    {
                        case "Damage":
                            return (TResult) (object) Damage;
                        default:
                            return base.GetProperty<TResult>(propertyName);
                    }
                }

                public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
                {
                    switch (methodName)
                    {
                        case "Prevent":
                            Prevented = true;
                            return (TResult) (object) new Void();
                        default:
                            return base.GetMethodResult<TResult>(methodName, arguments);
                    }
                }
            }

            public Damaged DamageInfo = new Damaged();
            
            public override TResult GetProperty<TResult>(string propertyName)
            {
                switch (propertyName)
                {
                    case "DamageInfo":
                        return (TResult) (object) DamageInfo;
                    default:
                        return base.GetProperty<TResult>(propertyName);
                }
            }
        }

        public override List<TResult> GetListProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(SnakeCircles):
                    return SnakeCircles.Cast<TResult>().ToList();
                default:
                    return base.GetListProperty<TResult>(propertyName);
            }
        }

        public override TResult GetProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Invulnerable):
                    return (TResult) (object) Invulnerable;
                default:
                    return base.GetProperty<TResult>(propertyName);
            }
        }

        public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
        {
            switch (methodName)
            {
                case nameof(Decrease):
                    Decrease((int) arguments[0]);
                    return (TResult) (object) new Void();
                case nameof(Increase):
                    Increase((int) arguments[0]);
                    return (TResult) (object) new Void();
                case nameof(Damage):
                    Damage();
                    return (TResult) (object) new Void();
                default:
                    return base.GetMethodResult<TResult>(methodName, arguments);
            }
        }

        public void Decrease(int size)
        {
            Snake = Snake.Decrease(size * Config.CircleOffset);
        }

        public void Increase(int size)
        {
            if (size < 0)
            {
                Decrease(-size);
            }
            else
            {
                Snake = Snake.Increase(size * Config.CircleOffset);
            }
        }

        public void Damage(int damage = 1)
        {
            if (!Invulnerable)
            {
                DamagedTime = _gameTime;
                _game.Damage(1);
            }
        }
    }
}