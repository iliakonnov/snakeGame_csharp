using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.MainGame;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace Snake
{
    /// <inheritdoc />
    public class Bonus : BonusBase
    {
        private readonly MainGame _game;
        private Texture2D _circle;

        private Color[] _colors;
        private IController _ctrl;
        private int _damage;
        private int _gameTime;
        private int _intersectStart;
        private Point[] _snakePoints;

        /// <summary>
        ///     Параметры бонуса
        /// </summary>
        public Config Config;

        /// <summary>
        ///     Время, когда последний раз был нанесен урон
        /// </summary>
        public int DamagedTime;

        /// <summary>
        ///     Сама змейка
        /// </summary>
        public SnakeModel Snake;

        /// <inheritdoc />
        public Bonus(Config cfg, MainGame game)
        {
            Config = cfg;
            _game = game;
        }

        /// <summary>
        ///     Неуязвима ли в данный момент змейка
        /// </summary>
        public bool Invulnerable { [Accessable] get; set; }

        /// <summary>
        ///     Все кружочки змейки
        /// </summary>
        public CircleF[] SnakeCircles { [Accessable] get; private set; } = { };

        /// <inheritdoc />
        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return new string[] { };
        }

        /// <inheritdoc />
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
                case AvailableControllers.Traditional:
                    _ctrl = new ControllerTraditional();
                    break;
                case AvailableControllers.Small:
                    _ctrl = new ControllerSmall(Config.TurnSize ?? 30);
                    break;
                default:
                    throw new ArgumentException("Unknown control type");
            }
        }

        /// <inheritdoc />
        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var control = _ctrl.Control(keyboard);
            if (control.ToTurn)
                Snake = control.ReplaceTurn
                    ? Snake.TurnAt(control.TurnDegrees)
                    : Snake.Turn(control.TurnDegrees);

            _gameTime = fullTime;

            Snake = Snake.ContinueMove(Config.Speed * time.ElapsedGameTime.Milliseconds / 1000f);

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
                if (head.Intersects(SnakeCircles[i]))
                    Damage();
            Invulnerable = _gameTime - DamagedTime < Config.DamageTimeout;

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

                if (oldEvents.Invulnerable != null) Invulnerable = (bool) oldEvents.Invulnerable;
            }

            _damage = 0;

            return snakeEvents;
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch sb)
        {
            var halfSize = Config.CircleSize / 2;

            for (var i = _snakePoints.Length - 1; i >= 0; i--)
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

        private static Texture2D CreateCircleTexture(int radius, GraphicsDevice gd)
        {
            var texture = new Texture2D(gd, radius, radius);
            var colorData = new Color[radius * radius];

            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (var x = 0; x < radius; x++)
            for (var y = 0; y < radius; y++)
            {
                var index = x * radius + y;
                var pos = new Vector2(x - diam, y - diam);
                if (pos.LengthSquared() <= diamsq)
                    colorData[index] = Color.White;
                else
                    colorData[index] = Color.Transparent;
            }

            texture.SetData(colorData);
            return texture;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
        {
            // ReSharper disable PossibleNullReferenceException (arguments can be null)
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
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        ///     Уменьшает размер змейки на указанное число кружочков
        ///     <paramref name="size" /> может быть меньше нуля, но в таком случае использется <see cref="Increase" />
        /// </summary>
        /// <param name="size">На сколько кружочков надо уменьшить змею</param>
        [Accessable]
        public void Decrease(int size)
        {
            if (size < 0)
                Increase(-size);
            else
                Snake = Snake.Decrease(size * Config.CircleOffset);
        }

        /// <summary>
        ///     Увеличивает размер змейки на указанное число кружочков
        ///     <paramref name="size" /> может быть меньше нуля, но в таком случае использется <see cref="Decrease" />
        /// </summary>
        /// <param name="size">На сколько кружочков надо уменьшить змею</param>
        [Accessable]
        public void Increase(int size)
        {
            if (size < 0)
                Decrease(-size);
            else
                Snake = Snake.Increase(size * Config.CircleOffset);
        }

        /// <summary>
        ///     Наносит змее указанный урон, если только она сейчас не неуязвима (<see cref="Invulnerable" />)
        /// </summary>
        /// <param name="damage"></param>
        [Accessable]
        public void Damage(int damage = 1)
        {
            if (!Invulnerable)
            {
                DamagedTime = _gameTime;
                Invulnerable = _gameTime - DamagedTime < Config.DamageTimeout;
                _game.Damage(damage);
            }
        }
    }
}