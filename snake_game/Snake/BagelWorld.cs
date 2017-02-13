using System;
using System.Linq;

namespace snake_game.Snake
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
			var x = ((int)pt.X) % Width;
			var y = ((int)pt.Y) % Height;
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
				return new[] { new Segment(bpA.NormalPoint, bpB.NormalPoint) };
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

		public Segment[] Normalize(SnakeModel s)
		{
			var pts = s.Points.ToArray();
			var segs = new Segment[pts.Length - 1];
			for (var i = 0; i < pts.Length - 1; i++)
			{
			    _cycle = 0;
				segs[i] = new Segment(pts[i], pts[i + 1]);
			}

			segs = segs.SelectMany(Normalize).ToArray();
			return segs;
		}

		Segment[] NormalizeFromInternal(BagelPoint bpA, BagelPoint bpB)
		{
			return Normalize(bpA, bpB, new[] { LeftSide, TopSide, RightSide, BottomSide });
		}

		Segment[] NormalizeFromBottomSide(BagelPoint bpA, BagelPoint bpB)
		{
			if (bpB.YP < 0)
			{
				bpB.YP += 1;
				bpA.YP += 1;

				return Normalize(bpA, bpB, new[] { LeftSide, RightSide, BottomSide }); // - TopSide
			}

			if (bpB.IsBottomSide && bpB.YP == 0)
			{
				// bpB на том же отрезке, что и bpA
				if (bpB.XP < 0)
				{
					var s1 = new Segment(
						new Point(0, 0),
						bpA.NormalPoint
						);

					return new[] { s1 }
						.Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
						.ToArray();
				}

				if (bpB.XP > 0)
				{
					var s1 = new Segment(
						bpA.NormalPoint,
						new Point(Width, 0)
						);

					bpB.XP -= 1;
					return new[] { s1 }
						.Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
						.ToArray();
				}

				throw new Exception();
			}

			return Normalize(bpA, bpB, new[] { LeftSide, TopSide, RightSide }); // - BottomSide 
		}
		Segment[] NormalizeFromLeftSide(BagelPoint bpA, BagelPoint bpB)
		{
			if (bpB.XP < 0)
			{
				bpB.XP += 1;
				bpA.XP += 1;

				return Normalize(bpA, bpB, new[] { LeftSide, TopSide, BottomSide }); // - RightSide
			}

			if (bpB.IsLeftSide && bpB.XP == 0)
			{
				// bpB на том же отрезке, что и bpA
				if (bpB.YP < 0)
				{
					var s1 = new Segment(
						new Point(0, 0),
						bpA.NormalPoint
						);

					return new[] { s1 }
						.Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
						.ToArray();
				}

				if (bpB.YP > 0)
				{
					var s1 = new Segment(
						bpA.NormalPoint,
						new Point(Width, 0)
						);

					bpB.YP -= 1;
					return new[] { s1 }
						.Concat(NormalizeFromVertex(BagelPoint.Vertex, bpB))
						.ToArray();
				}

				throw new Exception();
			}

			return Normalize(bpA, bpB, new[] { BottomSide, TopSide, RightSide }); // - LeftSide 
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
					return new[] { new Segment(
						GetStandardPoint(bpA),
						GetStandardPoint(bpB)) };
				if (bpB.IsLeftSide && bpB.YP > 0)
					return new[] { LeftSide };
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
					return new[] { new Segment(
						GetStandardPoint(bpA),
						GetStandardPoint(bpB)) };
				if (bpB.IsBottomSide && bpB.XP > 0)
					return new[] { BottomSide };
			}

			return Normalize(bpA, bpB, new[] { xside, yside });
		}
		Segment[] Normalize(BagelPoint bpA, BagelPoint bpB, Segment[] sides)
		{
		    if (!(bpA.XP == 0 || (bpA.XP == 1 && bpA.X == 0)))
		        throw new Exception($"Первая точка за пределами начальной страницы");
		    if (!(bpA.YP == 0 || (bpA.YP == 1 && bpA.Y == 0)))
		        throw new Exception($"Первая точка за пределами начальной страницы");

		    var a = GetStandardPoint(bpA);
			var b = GetStandardPoint(bpB);
			var s = new Segment(a, b);
		    if (IsInternal(a) && IsInternal(b))
		        return new[] {s};

            foreach (var side in sides)
			{
				var pt = MathUtils.Intersect(s, side);
				if (pt != null)
				{
				    if (pt.Equals(a))
				        throw new Exception();
				    if (pt.Equals(b))
				        throw new Exception();
				    var s1 = new Segment(a, pt);
					var s2 = new Segment(pt, b);
					return new[] { s1 }.Concat(Normalize(s2)).ToArray();
				}
			}

			throw new Exception($"Отрезок {s} не пересекается со сторонами мира");
		}

	    bool IsInternal(Point pt)
	    {
	        return pt.X >= 0 && pt.X <= Width &&
	               pt.Y >= 0 && pt.Y <= Height;
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

		    int _x;
		    public int X {
		        get { return _x; }
		        set
		        {
		            if (value < 0) throw  new ArgumentException();
		            if (value >= Width)throw  new ArgumentException();
		            _x = value;
		        }
		    }
		    int _y;

		    public int Y
		    {
		        get { return _y; }
		        set
		        {
		            if (value < 0) throw  new ArgumentException();
		            if (value >= Width)throw  new ArgumentException();
		            _y = value;
		        }
		    }
			public int XP { get; set; }
			public int YP { get; set; }

			public bool IsLeftSide => X == 0;

			public bool IsBottomSide => Y == 0;

			public bool IsVertex => IsLeftSide && IsBottomSide;

			public Point NormalPoint => new Point(X, Y);

		    public override string ToString()
		    {
		        return $"[{X}, {Y}] in Page [{XP}, {YP}]";
		    }
		}
		BagelPoint GetBagelPoint(Point pt)
		{
			var ix = (int)pt.X;
			var iy = (int)pt.Y;
			var hp = ix / Width;
		    var vp = iy / Height;

			if ((hp < 0 && ix % Width != 0) || (hp == 0 && ix < 0)) hp -= 1;
			if ((vp < 0 && iy % Height != 0) || (vp == 0 && iy < 0)) vp -= 1;

			return new BagelPoint
			{
				X = ix - hp * Width,
				Y = iy - vp * Height,
				XP = hp,
				YP = vp
			};
		}

		Point GetStandardPoint(BagelPoint pt)
		{
			return new Point(pt.X + Width * pt.XP, pt.Y + Height * pt.YP);
		}
	}
}
