using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Shapes;
using snake_game.Snake;
using Point = snake_game.Snake.Point;

namespace snake_game
{
    public class Polygon : IShapeF
    {
        public Vector2[] Vertices;
        public Segment[] Segments;
        public Line[] Lines;

        public Polygon(IEnumerable<Vector2> verts, Vector2? position=null)
        {
            var pos = position?? Vector2.Zero;
            
            var vertices = new List<Vector2>();
            var lines = new List<Line>();
            var segments = new List<Segment>();

            Point prevPoint = null;
            Point firstPoint = null;
            foreach (var vertex in verts)
            {
                var x = pos.X + vertex.X;
                var y = pos.Y + vertex.Y;
                vertices.Add(new Vector2(x, y));
                
                var newPoint = new Point(x, y);
                if (prevPoint != null)
                {
                    segments.Add(new Segment(prevPoint, newPoint));
                    lines.Add(MathUtils.StandardLine(prevPoint, newPoint));
                }
                else
                {
                    firstPoint = newPoint;
                }
                prevPoint = newPoint;
            }
            segments.Add(new Segment(prevPoint, firstPoint));
            lines.Add(MathUtils.StandardLine(prevPoint, firstPoint));

            Vertices = vertices.ToArray();
            Segments = segments.ToArray();
            Lines = lines.ToArray();
        }
        
        public Polygon(int n, int size, Vector2? position=null) : this(
            Enumerable.Range(0, n).Select(i =>
                new Vector2(
                    (float) (size * Math.Cos(2 * Math.PI * i / n)),
                    (float) (size * Math.Sin(2 * Math.PI * i / n))
                )
            ), position
        )
        {
            /*
            var verticies = new List<Vector2>();
            for (var i = 0; i < n; i++)
            {
                verticies.Add(new Vector2(
                    (float) (size / 2.0 * Math.Sin(Math.PI / n) * Math.Cos(2 * Math.PI * i / n)),
                    (float) (size / 2.0 * Math.Sin(Math.PI / n) * Math.Sin(2 * Math.PI * i / n))
                ));
            }
            Vertices = verticies.ToArray();
            */
        }

        public Polygon(PolygonF poly, Vector2? position=null) : this(poly.Vertices, position)
        {
        }

        public Polygon(RectangleF rect, Vector2? position=null) : this(rect.GetCorners(), position)
        {
        }
        
        public float Left => Vertices.Min(v => v.X);
        public float Top => Vertices.Min(v => v.Y);
        public float Right => Vertices.Max(v => v.X);
        public float Bottom => Vertices.Max(v => v.Y);

        public RectangleF GetBoundingRectangle()
        {
            var minX = Left;
            var minY = Top;
            var maxX = Right;
            var maxY = Bottom;
            
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public bool Contains(float x, float y)
        {
            var p = new Point(x, y);
            return Lines.All(line => line.Apply(p) > 0);
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        public PolygonF ToPolygonF()
        {
            return new PolygonF(Vertices);
        }

        public void PrettyDraw(SpriteBatch sb, Color color, float thickness)
        {
            foreach (var segment in Segments)
            {
                var newA = segment.MoveFromAToB(thickness/2);
                sb.DrawLine(new Vector2(newA.X, newA.Y), new Vector2(segment.B.X, segment.B.Y), color, thickness);
            }
        }
        
        public bool Intersects(float x, float y, float dist = 0)
        {
            var p = new Point(x, y);
            return Segments.Any(segment => MathUtils.Distance(segment, p) <= dist);
        }

        public bool Intersects(Segment seg)
        {
            return Segments.Any(segment => MathUtils.Intersect(seg, segment) != null);
        }
        
        public bool Intersects(Vector2 point)
        {
            return Intersects(point.X, point.Y);
        }

        public bool Intersects(Polygon polygon)
        {
            return polygon.Segments.Any(Intersects);
        }

        public bool Intersects(CircleF circle)
        {
            return Intersects(circle.Center.X, circle.Center.Y, circle.Radius);
        }

        public bool Intersects(RectangleF rect)
        {
            var corners = rect.GetCorners();
            var prev = new Point(corners[3].X, corners[3].Y);
            foreach (var corner in corners)
            {
                var newPoint = new Point(corner.X, corner.Y); 
                if (Intersects(new Segment(prev, newPoint)))
                {
                    return true;
                }
                prev = newPoint;
            }
            return false;
        }
    }
}