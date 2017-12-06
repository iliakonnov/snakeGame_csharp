using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using snake_game.Bonuses;
using snake_game.MainGame;
using snake_game.Snake;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace Snake
{
    public class Bonus : BonusBase
    {
        private Config _config;
        private SnakeModel _snake;
        private Color[] _colors;
        private int _intersectStart;
        private MainGame _game;
        private Texture2D _circle;
        private int _gameTime;
        private int _damagedTime;
        private IController _ctrl;
        private Point[] _snakePoints;

        public bool Invulnerable;
        public CircleF[] SnakeCircles = {};

        public Bonus(Config cfg, MainGame game)
        {
            _config = cfg;
            _game = game;
        }

        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return new string[] { };
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _snake =
                new SnakeModel(new Point(400, 150), 0).Increase(
                    _config.InitLen * _config.CircleOffset);

            if (_config.Colors == null)
            {
                var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

                var colors = new List<Color>();
                if (_config.HeadColor != null) colors.Add((Color) _config.HeadColor);
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.GetGetMethod() != null && propertyInfo.PropertyType == typeof(Color))
                    {
                        var col = (Color) propertyInfo.GetValue(null, null);
                        if (col != _config.HeadColor && col.A == 255)
                        {
                            colors.Add(col);
                        }
                    }
                }
                _colors = colors.ToArray();
            }
            else
            {
                _colors = _config.Colors;
            }

            _damagedTime = -_config.DamageTimeout;
            _circle = CreateCircleTexture(_config.CircleSize, graphicsDevice);

            _snakePoints = _snake.GetSnakeAsPoints(_config.CircleOffset);
            var headCenter = _snakePoints.First();
            var head = new CircleF(
                new Vector2(headCenter.X, headCenter.Y), _config.CircleSize
            );

            var halfSize = _config.CircleSize / 2;
            SnakeCircles = _snakePoints.Select(p => new CircleF(new Vector2(p.X, p.Y), halfSize)).ToArray();

            var i = 1;
            bool intersects;
            do
            {
                var current = new CircleF(
                    new Vector2(_snakePoints[i].X, _snakePoints[i].Y), _config.CircleSize
                );
                i++;
                intersects = head.Intersects(current);
            } while (intersects);
            _intersectStart = i;

            switch (_config.ControlType)
            {
                case "traditional":
                    _ctrl = new ControllerTraditional();
                    break;
                case "small":
                    _ctrl = new ControllerSmall(_config.TurnSize ?? 30);
                    break;
                default:
                    throw new ArgumentException("Unknown control type");
            }
        }

        public override Accessable Update(GameTime gameTime, int fullTime, KeyboardState keyboardState,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var control = _ctrl.Control(keyboardState);
            if (control.Turn.ToTurn)
            {
                _snake = control.Turn.ReplaceTurn
                    ? _snake.TurnAt(control.Turn.TurnDegrees)
                    : _snake.Turn(control.Turn.TurnDegrees);
            }

            _gameTime = fullTime;
            Invulnerable = _gameTime - _damagedTime < _config.DamageTimeout;

            _snake = _snake.ContinueMove(_config.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            var halfSize = _config.CircleSize / 2;
            var points = _snake.GetSnakeAsPoints(_config.CircleOffset);
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
            return null;
        }

        public override void Draw(SpriteBatch sb)
        {
            var halfSize = _config.CircleSize / 2;

            for (var i = 0; i < _snakePoints.Length; i++)
            {
                sb.Draw(
                    _circle,
                    new Vector2(
                        _snakePoints[i].X - halfSize,
                        _snakePoints[i].Y - halfSize
                    ),
                    Invulnerable
                        ? _config.DamageColor
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
            _snake = _snake.Decrease(size * _config.CircleOffset);
        }

        public void Increase(int size)
        {
            if (size < 0)
            {
                Decrease(-size);
            }
            else
            {
                _snake = _snake.Increase(size * _config.CircleOffset);
            }
        }

        public void Damage()
        {
            if (!Invulnerable)
            {
                _damagedTime = _gameTime;
                Invulnerable = true;
                _game.Damage(1);
            }
        }
    }
}