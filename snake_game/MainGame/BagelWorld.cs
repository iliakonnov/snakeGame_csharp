using System;
using System.Collections.Generic;
using System.Linq;
using snake_game.Utils;

namespace snake_game.MainGame
{
    public class BagelWorld
    {
        public BagelWorld(int height, int width)
        {
            Width = width;
            Height = height;
            BagelPoint.Width = width;
            BagelPoint.Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Segment LeftSide => new Segment(
            new Point(0, 0), new Point(0, Height)
        );

        public Segment TopSide => new Segment(
            new Point(0, Height), new Point(Width, Height)
        );

        public Segment RightSide => new Segment(
            new Point(Width, Height), new Point(Width, 0)
        );

        public Segment BottomSide => new Segment(
            new Point(0, 0), new Point(Width, 0)
        );

        public Point Normalize(Point pt)
        {
            var x = ((int) pt.X) % Width;
            var y = ((int) pt.Y) % Height;
            if (x < 0) x += Width;
            if (y < 0) y += Height;

            return new Point(x, y);
        }

        const int MAX_CYCLE = 10;
        int _cycle = 0;

        public Segment[] Normalize(Segment s)
        {
            if (_cycle++ > MAX_CYCLE)
            {
                throw new Exception();
                return new Segment[0];
            }
            var bpA = GetBagelPoint(s.A);
            var bpB = GetBagelPoint(s.B);
            if (bpA.XP == bpB.XP && bpA.YP == bpB.YP)
            {
                return new[] {new Segment(bpA.RelativePoint, bpB.RelativePoint)};
            }

            bpB.XP -= bpA.XP;
            bpA.XP = 0;
            bpB.YP -= bpA.YP;
            bpA.YP = 0;

            if (bpA.IsVertex)
                return NormalizeFromVertex(bpA, bpB);

            if (bpA.IsLeftSide)
                return NormalizeFromLeftSide(bpA, bpB);

            if (bpA.IsBottomSide)
                return NormalizeFromBottomSide(bpA, bpB);

            return NormalizeFromInternal(bpA, bpB);
        }

        public Segment[] Normalize(Point[] points)
        {
            var segs = new Segment[points.Length - 1];
            for (var i = 0; i < points.Length - 1; i++)
            {
                segs[i] = new Segment(points[i], points[i + 1]);
            }

            var result = new List<Segment>(segs.Length);
            foreach (var seg in segs)
            {
                _cycle = 0;
                result.AddRange(Normalize(seg));
            }

            //segs = segs.SelectMany(Normalize).ToArray();
            return result.ToArray();
        }

        Segment[] NormalizeFromInternal(BagelPoint bpA, BagelPoint bpB)
        {
            return Normalize(bpA, bpB, new[] {LeftSide, TopSide, RightSide, BottomSide});
        }

        Segment[] NormalizeFromBottomSide(BagelPoint bpA, BagelPoint bpB)
        {
            if (bpB.YP < 0)
            {
                bpB.YP += 1;
                bpA.YP += 1;

                return Normalize(bpA, bpB, new[] {LeftSide, RightSide, BottomSide}); // - TopSide
            }

            if (bpB.IsBottomSide && bpB.YP == 0)
            {
                // bpB на том же отрезке, что и bpA
                if (bpB.XP < 0)
                {
                    var s1 = new Segment(
                        new Point(0, 0),
                        bpA.RelativePoint
                    );

                    return new[] {s1}
                        .Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
                        .ToArray();
                }

                if (bpB.XP > 0)
                {
                    var s1 = new Segment(
                        bpA.RelativePoint,
                        new Point(Width, 0)
                    );

                    bpB.XP -= 1;
                    return new[] {s1}
                        .Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
                        .ToArray();
                }

                throw new Exception();
            }

            return Normalize(bpA, bpB, new[] {LeftSide, TopSide, RightSide}); // - BottomSide 
        }

        Segment[] NormalizeFromLeftSide(BagelPoint bpA, BagelPoint bpB)
        {
            if (bpB.XP < 0)
            {
                bpB.XP += 1;
                bpA.XP += 1;

                return Normalize(bpA, bpB, new[] {LeftSide, TopSide, BottomSide}); // - RightSide
            }

            if (bpB.IsLeftSide && bpB.XP == 0)
            {
                // bpB на том же отрезке, что и bpA
                if (bpB.YP < 0)
                {
                    var s1 = new Segment(
                        new Point(0, 0),
                        bpA.RelativePoint
                    );

                    return new[] {s1}
                        .Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
                        .ToArray();
                }

                if (bpB.YP > 0)
                {
                    var s1 = new Segment(
                        bpA.RelativePoint,
                        new Point(Width, 0)
                    );

                    bpB.YP -= 1;
                    return new[] {s1}
                        .Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
                        .ToArray();
                }

                throw new Exception();
            }

            return Normalize(bpA, bpB, new[] {BottomSide, TopSide, RightSide}); // - LeftSide 
        }

        Segment[] NormalizeFromVertex(BagelPoint bpA, BagelPoint bpB)
        {
            var yside = RightSide;
            var xside = TopSide;
            if (bpB.XP < 0)
            {
                bpB.XP += 1;
                bpA.XP += 1;
                yside = LeftSide;
            }
            else if (bpB.XP == 0)
            {
                if (bpB.YP == 0)
                    return new[]
                    {
                        new Segment(
                            bpA.AbsolutePoint,
                            bpB.AbsolutePoint)
                    };
                if (bpB.IsLeftSide && bpB.YP > 0)
                    return new[] {LeftSide};
            }

            if (bpB.YP < 0)
            {
                bpB.YP += 1;
                bpA.YP += 1;
                xside = BottomSide;
            }
            else if (bpB.YP == 0)
            {
                if (bpB.XP == 0)
                    return new[]
                    {
                        new Segment(
                            bpA.AbsolutePoint,
                            bpB.AbsolutePoint)
                    };
                if (bpB.IsBottomSide && bpB.XP > 0)
                    return new[] {BottomSide};
            }

            return Normalize(bpA, bpB, new[] {xside, yside});
        }

        Segment[] Normalize(BagelPoint bpA, BagelPoint bpB, Segment[] sides)
        {
            if (!(bpA.XP == 0 || (bpA.XP == 1 && bpA.X == 0)))
                throw new Exception($"Первая точка за пределами начальной страницы");
            if (!(bpA.YP == 0 || (bpA.YP == 1 && bpA.Y == 0)))
                throw new Exception($"Первая точка за пределами начальной страницы");

            var a = bpA.AbsolutePoint;
            var b = bpB.AbsolutePoint;
            var s = new Segment(a, b);
            if (IsInRectangle(a) && IsInRectangle(b))
                return new[] {s};

            foreach (var side in sides)
            {
                var pt = MathUtils.Intersect(s, side);
                if (pt != null)
                {
                    var s1 = new Segment(a, pt);
                    var s2 = new Segment(b, pt);
                    if (pt.Equals(a))
                        return Normalize(s2);
                    if (pt.Equals(b))
                        return new[] {s1};
                    return new[] {s1}.Concat(Normalize(s2)).ToArray();
                }
            }

            throw new Exception($"Отрезок {s} не пересекается со сторонами мира");
        }

        bool IsInRectangle(Point pt)
        {
            return pt.X >= 0 && pt.X <= Width &&
                   pt.Y >= 0 && pt.Y <= Height;
        }

        bool IsInternal(Point pt)
        {
            return pt.X >= 0 && pt.X < Width &&
                   pt.Y >= 0 && pt.Y < Height;
        }

        class BagelPoint
        {
            public static BagelPoint Vertex => new BagelPoint
            {
                X = 0,
                Y = 0,
                XP = 0,
                YP = 0
            };

            public static int Width;
            public static int Height;

            float _x;

            public float X
            {
                get { return _x; }
                set
                {
                    if (value < 0) throw new ArgumentException();
                    if (value >= Width) throw new ArgumentException();
                    _x = value;
                }
            }

            float _y;

            public float Y
            {
                get { return _y; }
                set
                {
                    if (value < 0) throw new ArgumentException();
                    if (value >= Width) throw new ArgumentException();
                    _y = value;
                }
            }

            public int XP { get; set; }
            public int YP { get; set; }

            public bool IsLeftSide => X == 0;

            public bool IsBottomSide => Y == 0;

            public bool IsVertex => IsLeftSide && IsBottomSide;

            public Point RelativePoint => new Point(X, Y);
            public Point AbsolutePoint => new Point(X + Width * XP, Y + Height * YP);

            public override string ToString()
            {
                return $"[{X}, {Y}] in Page [{XP}, {YP}]";
            }
        }

        BagelPoint GetBagelPoint(Point pt)
        {
            int xp = 0;
            int yp = 0;
            while (!IsInternal(pt))
            {
                int xd = 0, yd = 0;
                if (pt.X < 0)
                {
                    xp -= 1;
                    xd = Width;
                }
                else if (pt.X >= Width)
                {
                    xp += 1;
                    xd = -Width;
                }

                if (pt.Y < 0)
                {
                    yp -= 1;
                    yd = Height;
                }
                else if (pt.Y >= Height)
                {
                    yp += 1;
                    yd = -Height;
                }
                pt = pt.Add(new Point(xd, yd));
            }

            return new BagelPoint
            {
                X = pt.X,
                Y = pt.Y,
                XP = xp,
                YP = yp
            };
        }
    }
}