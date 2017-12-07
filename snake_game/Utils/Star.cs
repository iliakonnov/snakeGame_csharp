using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace snake_game.Utils
{
    public class StarFactory
    {
        private List<Segment> _outer;
        private List<Triangle> _inner;

        // {n/m}: https://en.wikipedia.org/wiki/Star_polygon
        public StarFactory(int n, int m, int radius)
        {
            var additional = 0f;
            if (n % 2 != 0)
            {
                additional = (float) Math.PI / -2;
            }

            // Находит вершины
            var vertexes = new List<Point>();
            var angle = (float) (2 * Math.PI / n);
            for (var i = 0; i < n; i++)
            {
                vertexes.Add(Point.FromRadians(additional + i * angle, radius));
            }

            // Находит "внешние" пересечния
            var half = angle / 2;
            var r = (float) (radius * Math.Cos(Math.PI * (m - 1) / n) * (1 / Math.Sin(Math.PI * m / n)));
            var points = new List<Point>();
            for (var i = 0; i < n; i++)
            {
                points.Add(Point.FromRadians(additional + half + i * angle, r));
            }

            // Строит треугольники
            _inner = new List<Triangle>();
            var center = new Point(0, 0);
            for (var i = 0; i < n; i++)
            {
                _inner.Add(new Triangle(
                    points[i], vertexes[i], center
                ));
            }

            // Строит внешние стороны
            _outer = new List<Segment>();
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

        public Star GetStar(Point position)
        {
            var outer = _outer.Select(seg => new Segment(seg.A.Add(position), seg.B.Add(position))).ToList();
            var inner = _inner.Select(triangle =>
                new Triangle(triangle.A.Add(position), triangle.B.Add(position), triangle.C.Add(position))).ToList();
            return new Star(outer, inner);
        }
    }

    public class Star
    {
        private List<Segment> _outer;
        private List<Triangle> _inner;
        private List<Line> _lines;

        public Star(List<Segment> outer, List<Triangle> inner)
        {
            _outer = outer;
            _inner = inner;
            _lines = outer.Select(seg => MathUtils.StandardLine(seg.A, seg.B)).ToList();
        }

        public void BorderDraw(SpriteBatch sb, Color color, float thickness)
        {
            foreach (var segment in _outer)
            {
                sb.DrawLine(new Vector2(segment.A.X, segment.A.Y), new Vector2(segment.B.X, segment.B.Y), color,
                    thickness);
            }
        }

        public void FillDraw(SpriteBatch sb, Color color)
        {
            foreach (var triangle in _inner)
            {
                triangle.Draw(sb, color);
            }
        }
    }

    public class Triangle
    {
        private Color _color;

        public Point A;
        public Point B;
        public Point C;

        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        public void Draw(SpriteBatch sb, Color color)
        {
            throw new NotImplementedException("Сделать отрисовку треугольника");
        }
    }
}