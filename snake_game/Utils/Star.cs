using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace snake_game.Utils
{
    public class StarFactory
    {
        private IList<Segment> _outer;
        private IList<Triangle> _triangles;
        private Texture2D _inner;
        private int _radius;

        // {n/m}: https://en.wikipedia.org/wiki/Star_polygon
        public StarFactory(int n, int m, int radius, GraphicsDevice gd)
        {
            m++;
            _radius = radius;
            var additional = 0f;
            if (n % 2 != 0)
            {
                additional = (float) Math.PI / -2;
            }

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

            float innerRadius;
            if (m > 2) // Звезда, не многоугольник
            {
                innerRadius = (float) (radius * Math.Cos(Math.PI * (m - 1) / n) * (1 / Math.Sin(Math.PI * m / n)));
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
                {
                    _outer.Add(new Segment(
                        points[i], vertexes[i]
                    ));
                }
                _outer.Add(new Segment(
                    points[n - 1], vertexes[0]
                ));
                for (var i = 1; i < n; i++)
                {
                    _outer.Add(new Segment(
                        points[i - 1], vertexes[i]
                    ));
                }
            }
            else // Обычный многоугольник, не звезда
            {
                innerRadius = 0;
                _outer.Add(new Segment(
                    vertexes[n - 1], vertexes[0]
                ));
                for (var i = 1; i < n; i++)
                {
                    _outer.Add(new Segment(
                        vertexes[i - 1], vertexes[i]
                    ));
                }
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
            {
                for (var y = -radius; y < radius; y++)
                {
                    point = new Point(x, y);
                    var inside = CheckIsPointInsideStar(point, outerRadiusSquare, _triangles);
                    var index = (y + radius) * doubleRadius + x + radius;
                    if (inside)
                    {
                        textureColors[index] = Color.White;
                    }
                    else
                    {
                        textureColors[index] = Color.Transparent;
                    }
                }
            }
            _inner.SetData(textureColors);
        }

        public static bool CheckIsPointInsideStar(Point point, int outerRadiusSquare, IEnumerable<Triangle> triangles)
        {
            var center = new Point(0, 0);
            var seg = new Segment(center, point);
            var radiusSquared = point.X * point.X + point.Y * point.Y;

            var inside =
                radiusSquared <= outerRadiusSquare && (
                    (int) point.X == 0 && (int) point.Y == 0 ||
                    triangles.Any(t => t.Contains(point))
                );
            return inside;
        }

        public Star GetStar(Point position)
        {
            var star = new Star(_radius, _triangles, _outer, _inner)
            {
                Position = position
            };
            return star;
        }

        public Star GetStar()
        {
            return new Star(_radius, _triangles, _outer, _inner);
        }

        public void Check()
        {
            using (var s = File.OpenWrite("texture.png"))
            {
                _inner.SaveAsPng(s, _radius * 2, _radius * 2);
            }
        }
    }

    public class Star
    {
        private IEnumerable<Triangle> _triangles;
        private readonly IEnumerable<Triangle> _trianglesZero;
        private IEnumerable<Segment> _outer;
        private readonly IEnumerable<Segment> _outerZero;
        private Texture2D _inner;
        private readonly int _radius;

        private readonly int _outerRadiusSquare;

        private Point _position;

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

        public Star(int radius, IEnumerable<Triangle> triangles, IEnumerable<Segment> outer, Texture2D inner)
        {
            _trianglesZero = triangles;
            _outerZero = outer;
            _inner = inner;
            _radius = radius;

            _outerRadiusSquare = radius * radius;

            Position = new Point(0, 0);
        }

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

        public void BorderDraw(SpriteBatch sb, Color color, float thickness)
        {
            _outer.ToList().ForEach(segment => sb.DrawLine(new Vector2(segment.A.X, segment.A.Y),
                new Vector2(segment.B.X, segment.B.Y), color,
                thickness));
        }

        public void FillDraw(SpriteBatch sb, Color color)
        {
            sb.Draw(_inner, new Vector2(_position.X - _radius, _position.Y - _radius), color);
        }

        public bool Intersects(Segment segment)
        {
            return _outer.Any(seg => MathUtils.Intersect(segment, seg) != null);
        }

        public bool Contains(Point point)
        {
            var posX = (int) _position.X;
            var posY = (int) _position.Y;
            var x = (int) point.X - posX;
            var y = (int) point.Y - posY;

            return
                StarFactory.CheckIsPointInsideStar(new Point(x, y), _outerRadiusSquare, _trianglesZero);
        }
    }
}