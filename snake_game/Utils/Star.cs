using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace snake_game.Utils
{
    /// <summary>
    ///     Создаёт звёзды (<see cref="Star" />). Звезда это звездчатый многоугольник (
    ///     <see href="https://en.wikipedia.org/wiki/Star_polygon" />).
    ///     Создание <see cref="StarFactory" /> -- дорогая операция, рекомендуется создавать только один экземпляр на один тип
    ///     звёзд
    /// </summary>
    public class StarFactory
    {
        private readonly Texture2D _inner;
        private readonly IList<Segment> _outer;
        private readonly int _radius;
        private readonly IList<Triangle> _triangles;

        /// <inheritdoc />
        /// <param name="n">Количество вершин</param>
        /// <param name="m">Каждая вершина будет соединена с с каждой m-ной вершиной по часовой стрелке</param>
        /// <param name="radius">Радиус звезды, от центра до внешних вершин</param>
        /// <param name="gd">Графическое устройство</param>
        public StarFactory(int n, int m, int radius, GraphicsDevice gd)
        {
            m++;
            _radius = radius;
            var additional = 0f;
            if (n % 2 != 0) additional = (float) Math.PI / -2;

            Point point;

            // Находит вершины
            var vertexes = new List<Point>();
            var angle = (float) (2 * Math.PI / n);
            for (var i = 0; i < n; i++)
            {
                point = Point.FromRadians(additional + i * angle, radius);
                vertexes.Add(point);
            }

            _outer = new List<Segment>();

            if (m > 2) // Звезда, не многоугольник
            {
                var innerRadius = (float) (radius * Math.Cos(Math.PI * (m - 1) / n) * (1 / Math.Sin(Math.PI * m / n)));
                // Находит "внешние" пересечения
                var half = angle / 2;
                var points = new List<Point>();
                for (var i = 0; i < n; i++)
                {
                    point = Point.FromRadians(additional + half + i * angle, innerRadius);
                    points.Add(point);
                }

                // Строит внешние стороны
                for (var i = 0; i < n; i++)
                    _outer.Add(new Segment(
                        points[i], vertexes[i]
                    ));

                _outer.Add(new Segment(
                    points[n - 1], vertexes[0]
                ));
                for (var i = 1; i < n; i++)
                    _outer.Add(new Segment(
                        points[i - 1], vertexes[i]
                    ));
            }
            else // Обычный многоугольник, не звезда
            {
                _outer.Add(new Segment(
                    vertexes[n - 1], vertexes[0]
                ));
                for (var i = 1; i < n; i++)
                    _outer.Add(new Segment(
                        vertexes[i - 1], vertexes[i]
                    ));
            }

            var center = new Point(0, 0);
            // Строит треугольники
            _triangles = _outer.Select(seg => new Triangle(seg.A, seg.B, center)).ToList();

            // Строит текстуру
            var doubleRadius = radius * 2;
            var outerRadiusSquare = radius * radius;
            _inner = new Texture2D(gd, doubleRadius, doubleRadius);
            var textureColors = new Color[doubleRadius * doubleRadius];
            for (var x = -radius; x < radius; x++)
            for (var y = -radius; y < radius; y++)
            {
                point = new Point(x, y);
                var inside = CheckIsPointInsideStar(point, outerRadiusSquare, _triangles);
                var index = (y + radius) * doubleRadius + x + radius;
                if (inside)
                    textureColors[index] = Color.White;
                else
                    textureColors[index] = Color.Transparent;
            }

            _inner.SetData(textureColors);
        }

        /// <summary>
        ///     Проверяет, находится ли точка внутри звезды
        /// </summary>
        /// <param name="point">Проверяемая точка</param>
        /// <param name="outerRadiusSquare">Внешний радиус звезды, возведенный в квадрат</param>
        /// <param name="triangles">Треугольники, на которые разбита звезда</param>
        /// <returns></returns>
        public static bool CheckIsPointInsideStar(Point point, int outerRadiusSquare, IEnumerable<Triangle> triangles)
        {
            var radiusSquared = point.X * point.X + point.Y * point.Y;

            var inside =
                radiusSquared <= outerRadiusSquare && (
                    (int) point.X == 0 && (int) point.Y == 0 ||
                    triangles.Any(t => t.Contains(point))
                );
            return inside;
        }

        /// <summary>
        ///     Возвращает звезду и перемещает её в указанные координаты
        /// </summary>
        /// <param name="position">Координаты звезды</param>
        /// <returns>Звезда в координатах</returns>
        public Star GetStar(Point position)
        {
            var star = new Star(_radius, _triangles, _outer, _inner)
            {
                Position = position
            };
            return star;
        }

        /// <summary>
        ///     Возвращает звезду
        /// </summary>
        /// <returns>Звезда</returns>
        public Star GetStar()
        {
            return new Star(_radius, _triangles, _outer, _inner);
        }
    }

    /// <summary>
    ///     Содержит уже созданную звезду
    /// </summary>
    public class Star
    {
        private readonly Texture2D _inner;

        private readonly int _outerRadiusSquare;
        private readonly IEnumerable<Segment> _outerZero;
        private readonly int _radius;
        private readonly IEnumerable<Triangle> _trianglesZero;
        private IEnumerable<Segment> _outer;

        private Point _position;
        private IEnumerable<Triangle> _triangles;

        /// <summary>
        ///     Создаёт звезду. Не рекомендуется использовать самому, нужно создавать звезды при помощи <see cref="StarFactory" />
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="triangles"></param>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        public Star(int radius, IEnumerable<Triangle> triangles, IEnumerable<Segment> outer, Texture2D inner)
        {
            _trianglesZero = triangles;
            _outerZero = outer;
            _inner = inner;
            _radius = radius;

            _outerRadiusSquare = radius * radius;

            Position = new Point(0, 0);
        }

        /// <summary>
        ///     Координаты центра звезды
        /// </summary>
        public Point Position
        {
            get => _position;
            set
            {
                _triangles = _trianglesZero.Select(t => t.Move(value)).ToList();
                _outer = _outerZero.Select(seg => new Segment(seg.A.Add(value), seg.B.Add(value))).ToList();
                _position = value;
            }
        }

        /// <summary>
        ///     Отрисовывает границы звезды и соединяет каждый её угол с центром
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color">Цвет, которым нужно рисовать линии</param>
        /// <param name="thickness">Толщина линий</param>
        public void PrettyDraw(SpriteBatch sb, Color color, float thickness)
        {
            _triangles.SelectMany(t =>
                {
                    var ss = t.ToSegments();
                    return new[] {ss.Item1, ss.Item2, ss.Item3};
                })
                .ToList()
                .ForEach(segment => sb.DrawLine(new Vector2(segment.A.X, segment.A.Y),
                    new Vector2(segment.B.X, segment.B.Y), color,
                    thickness));
        }

        /// <summary>
        ///     Отрисовывает только границы звезды
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color">Цвет, которым нужно рисовать линии</param>
        /// <param name="thickness">Толщина линий</param>
        public void BorderDraw(SpriteBatch sb, Color color, float thickness)
        {
            _outer.ToList().ForEach(segment => sb.DrawLine(new Vector2(segment.A.X, segment.A.Y),
                new Vector2(segment.B.X, segment.B.Y), color,
                thickness));
        }

        /// <summary>
        ///     Отрисовывает полностью закрашенную звезду
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color">Цвет звезды</param>
        public void FillDraw(SpriteBatch sb, Color color)
        {
            sb.Draw(_inner, new Vector2(_position.X - _radius, _position.Y - _radius), color);
        }

        /// <summary>
        ///     Проверяет, пересекает ли отрезок границы звезды
        /// </summary>
        /// <param name="segment">Отрезок, который нужно проверить</param>
        /// <returns>Результат проверки</returns>
        public bool Intersects(Segment segment)
        {
            return _outer.Any(seg => MathUtils.Intersect(segment, seg) != null);
        }

        /// <summary>
        ///     Проверяет, находится ли точка в пределах или на границах звезды
        /// </summary>
        /// <param name="point">Точка, которую необходимо проверить</param>
        /// <returns>Результат проверки</returns>
        public bool Contains(Point point)
        {
            var posX = (int) _position.X;
            var posY = (int) _position.Y;
            var x = (int) point.X - posX;
            var y = (int) point.Y - posY;

            return
                StarFactory.CheckIsPointInsideStar(new Point(x, y), _outerRadiusSquare, _trianglesZero);
        }

        /// <summary>
        ///     Проверяет, пересекает ли окружность границы звезды.
        ///     Внимание, проверка осуществляется на основе описанного квадрата вокруг окружности
        /// </summary>
        /// <param name="circle">Окружность, которую нужно проверить</param>
        /// <returns>Результат проверки</returns>
        public bool Intersects(CircleF circle)
        {
            var rect = circle.ToRectangle();
            var rectSegments = new[]
            {
                new Segment(new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top)),
                new Segment(new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom)),
                new Segment(new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom)),
                new Segment(new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom))
            };
            return rectSegments.Any(Intersects);
        }
    }
}