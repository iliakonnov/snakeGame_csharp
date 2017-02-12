using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace snake_game.Snake
{
	/// <summary>
	/// Модель змеи, состоящей из отрезков. Голова может двигаться в любом направлении.
	/// </summary>
	public class SnakeModel
	{
		const double EPS = 1e-6;

		// Длина змеи
		double _length;
		// точки, задающие повороты змеи. Первый - голова
		Point[] _nodes;
		// направление движения головы в градусах
	    double _headDirection;

	    SnakeModel() { }
		public SnakeModel(Point head, int direction)
		{
			_length = 0;
			_nodes = new[] { head };
			Direction = direction;
		}

		public IReadOnlyCollection<Point> Points => new ReadOnlyCollection<Point>(_nodes);

	    public double Direction
	    {
	        get { return _headDirection; }
	        set
	        {
	            if (value < 0) value += 360;
	            if (value >= 360) value -= 360;
	            _headDirection = value;
	        }
	    }

	    public SnakeModel ContinueMove(int s)
		{
			if (_nodes.Length == 0)
				throw new Exception();

			var head = _nodes[0];
			var pt = Point.FromPolar(_headDirection, s);
			if (_nodes.Length == 1)
			{
				return new SnakeModel
				{
					_nodes = new[] { head.Add(pt) },
					_length = 0,
					Direction = _headDirection
				};
			}

			var nodes = _nodes.ToArray();
			nodes[0] = head.Add(pt);
			return SetLength(_length, nodes, _headDirection);
		}

		public SnakeModel Turn(double alpha)
		{
			var nodes = _nodes;
			if (_nodes.Length > 1)
			{
				var head = _nodes[0];
				nodes = new[] { head }.Concat(_nodes).ToArray();
			}

			return new SnakeModel
			{
				_nodes = nodes,
				_length = _length,
				Direction = _headDirection + alpha
			};

		}

		public SnakeModel Increase(int s)
		{
			return SetLength(_length + s, _nodes, _headDirection);
		}

		public SnakeModel Decrease(int s)
		{
			if (_length < s) throw new ArgumentException();
			if (Math.Abs(_length - s) < EPS)
				return new SnakeModel
				{
					_nodes = new[] { _nodes[0] },
					_length = 0,
					Direction = _headDirection
				};

			return SetLength(_length - s, _nodes, _headDirection);
		}

		static SnakeModel SetLength(double length, Point[] nodes, double headDirection)
		{
			if (nodes.Length == 0) throw new Exception();

			if (nodes.Length == 1)
			{
				var head = nodes[0];
				var pt = Point.FromPolar(headDirection, -length);
				return new SnakeModel
				{
					_nodes = new[] { head, pt },
					_length = length,
					Direction = headDirection
				};
			}
		    if (seg == null)
		    {
		        throw new Exception();
		    }

			int i;
			double c;
			Segment tail = null;
			for (i = 1, c = 0; i < nodes.Length && c < length; i++)
			{
				tail = new Segment(nodes[i], nodes[i - 1]);
				c += tail.Length;
			}
			var newNodes = nodes.Take(i).ToArray();

			if (Math.Abs(c - length) < EPS)
			{
				return new SnakeModel
				{
					_nodes = nodes,
					_length = c,
					Direction = headDirection
				};
			}
			else
			{
				// надо укоротить или удлинить
				var pt = tail.MoveFromAToB(c - length);
				nodes[nodes.Length - 1] = pt;
				return new SnakeModel
				{
					_nodes = nodes,
					_length = length,
					Direction = headDirection
				};
			}
		}
	}
}
