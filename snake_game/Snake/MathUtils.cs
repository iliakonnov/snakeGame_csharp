using System;
using System.Linq;

namespace snake_game.Snake
{
	/// <summary>
	/// Точка внутри плоскости
	/// </summary>
	public class Point
	{
		const double EPSILON = 1e-16;
		public Point(double x, double y)
		{
			_x = x;
			_y = y;
		}

		public static Point FromPolar(double alpha, double r)
		{
			return new Point(r * Math.Cos(alpha), r * Math.Sin(alpha));
		}

		readonly double _x;
		public double X { get { return _x; } }
		readonly double _y;
		public double Y { get { return _y; } }

		public Point Add(Point pt)
		{
			return new Point(X + pt.X, Y + pt.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as Point);
		}
		public bool Equals(Point obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Math.Abs(_x - obj._x) < EPSILON &&
				Math.Abs(_y - obj._y) < EPSILON;
		}

		public override int GetHashCode()
		{
			return ((_x + _y) + Math.Sign(_x + _y)).GetHashCode();
		}

		public override string ToString()
		{
			return $"({_x}, {_y})";
		}
	}

	/// <summary>
	/// Каноническое уравнение прямой
	/// </summary>
	public class Line
	{
		// на таком отличии 1.GetHashCode() не замечает разницы 
		const double EPSILON = 1e-16;
		/// <summary>
		/// Прямая ax + by + c = 0
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		public Line(double a, double b, double c)
		{
			// Гарантирую, что 
			// |a| + |b| = 1
			// a > 0 или
			// a = 0 и b > 0
			var n = Math.Abs(a) + Math.Abs(b);
			if (n < EPSILON)
				throw new ArgumentException();

			if (Math.Abs(a) > EPSILON)
			{
				if (a < 0)
					n = -n;
			}
			else // if (Math.Abs(a) <= EPSILON)
			{
				if (b < 0)
					n = -n;
			}

			A = a / n;
			B = b / n;
			C = c / n;
			if (A + C > EPSILON) _hash = (1 + A + C).GetHashCode();
			else if (A + C < EPSILON) _hash = (-1 + A + C).GetHashCode();
			else _hash = 0;
		}
		public double A { get; private set; }
		public double B { get; private set; }
		public double C { get; private set; }

		public double Apply(Point a)
		{
			return A * a.X + B * a.Y + C;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null)) return false;
			if (ReferenceEquals(obj, this)) return true;
			return Equals(obj as Line);
		}
		public bool Equals(Line obj)
		{
			if (ReferenceEquals(obj, null)) return false;
			if (ReferenceEquals(obj, this)) return true;

			return Parallels(obj) &&
				(Math.Abs(C - obj.C) <= EPSILON);
		}

		public bool Parallels(Line obj)
		{
			if (ReferenceEquals(obj, null)) return false;
			if (ReferenceEquals(obj, this)) return true;

			//var D = A * obj.B - obj.A * B;
			//return Math.Abs(D) < EPSILON;

			return (Math.Abs(A - obj.A) <= EPSILON) &&
				(Math.Abs(B - obj.B) <= EPSILON);
		}

		readonly int _hash;
		public override int GetHashCode()
		{
			return _hash;
		}

		public override string ToString()
		{
			return $"{A}x + {B}y + {C} = 0";
		}
	}

	/// <summary>
	/// Ориентированный отрезок
	/// </summary>
	public class Segment
	{
		public Segment(Point a, Point b)
		{
			B = b;
			A = a;
		}

		public Point A { get; private set; }
		public Point B { get; private set; }

		public double Length
		{
			get
			{
				var x = A.X - B.X;
				var y = A.Y - B.Y;
				return Math.Sqrt(x * x + y * y);
			}
		}

		public Point MoveFromAToB(double s)
		{
			var p = s / Length;
			return new Point(
				A.X + p * (B.X - A.X),
				A.Y + p * (B.Y - A.Y)
				);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as Segment);
		}

		public bool Equals(Segment obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return A.Equals(obj.A) && B.Equals(obj.B);
		}

		public override int GetHashCode()
		{
			return unchecked(A.GetHashCode() << 1) ^ B.GetHashCode();
		}

		public override string ToString()
		{
			return $"[{A}; {B}]";
		}
	}
	public static class MathUtils
	{
		const double EPSILON = 1e-16;
		/// <summary>
		/// Получить стандартную форму прямой, проходящей через заданные точки
		/// X1 * x + X2 * y + X3 = 0
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Line StandardLine(Point a, Point b)
		{
			return new Line(a.Y - b.Y, b.X - a.X, a.X * b.Y - b.X * a.Y);
		}

		/// <summary>
		/// Точка пересечения двух прямых, заданных в стандартной форме.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point Intersect(Line a, Line b)
		{
			var D = a.A * b.B - b.A * a.B;

			var x = a.B * b.C - a.C * b.B;
			var y = a.C * b.A - a.A * b.C;
			return new Point(x / D, y / D);
		}

		/// <summary>
		/// Находятся ли точки по одну сторону от прямой.
		/// -1 - по разные стороны
		/// 0 - одна или обе лежат на прямой
		/// 1 - по одну сторону
		/// </summary>
		/// <param name="line"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int AreOnTheSameSide(Line line, Point a, Point b)
		{
			var aa = line.Apply(a);
			if (Math.Abs(aa) < EPSILON) return 0;
			var bb = line.Apply(b);
			if (Math.Abs(bb) < EPSILON) return 0;
			var signA = Math.Sign(aa);
			var signB = Math.Sign(bb);
			return signA * signB;
		}

		/// <summary>
		/// Пересекаются ли отрезки. Если совпадают более, чем в одной точке,
		/// то возвращается произвольная точка пересечения.
		/// Если не пересекаются, то null
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Point Intersect(Segment a, Segment b)
		{
			if (a.A.Equals(b.A) || a.A.Equals(b.B)) return a.A;
			if (a.B.Equals(b.A) || a.B.Equals(b.B)) return a.B;

			var aXmin = new[] { a.A.X, a.B.X }.Min();
			var aXmax = new[] { a.A.X, a.B.X }.Max();
			var aYmin = new[] { a.A.Y, a.B.Y }.Min();
			var aYmax = new[] { a.A.Y, a.B.Y }.Max();

			var bXmin = new[] { b.A.X, b.B.X }.Min();
			var bXmax = new[] { b.A.X, b.B.X }.Max();
			var bYmin = new[] { b.A.Y, b.B.Y }.Min();
			var bYmax = new[] { b.A.Y, b.B.Y }.Max();

			if (new[] { aXmin, bXmin }.Max() > new[] { aXmax, bXmax }.Min() ||
				new[] { aYmin, bYmin }.Max() > new[] { aYmax, bYmax }.Min())
				// если прямоугольники не пересекаются
				return null;

			var lineA = StandardLine(a.A, a.B);
			var lineB = StandardLine(b.A, b.B);
			if (lineA.Equals(lineB))
			{
				// на одной прямой и прямоугольники пересекаются
				if (a.A.X >= bXmin && a.A.X <= bXmax)
					// точка a.A внутри b
					return a.A;

				if (a.B.X >= bXmin && a.B.X <= bXmax)
					// точка a.B внутри b
					return a.B;

				// b внутри a
				return b.A;
			}

			if (lineA.Parallels(lineB))
				// параллельные прямые
				return null;

			var sideA = AreOnTheSameSide(lineA, b.A, b.B);
			if (sideA > 0) return null;
			var sideB = AreOnTheSameSide(lineB, a.A, a.B);
			if (sideB > 0) return null;

			if (sideA == 0)
			{
				// b касается a
				if (Math.Abs(lineA.Apply(b.A)) < EPSILON) return b.A;
				return b.B;
			}

			if (sideB == 0)
			{
				// a касается b
				if (Math.Abs(lineB.Apply(a.A)) < EPSILON) return a.A;
				return a.B;
			}

			return Intersect(lineA, lineB);
		}

	}
}
