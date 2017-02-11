using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
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
		private SnakeModel() { }
		public SnakeModel(Point head, int direction)
		{
			_length = 0;
			_nodes = new[] { head };
			_headDirection = direction;
		}

		public IReadOnlyCollection<Point> Points
		{
			get { return new ReadOnlyCollection<Point>(_nodes); }
		}

		public double Direction { get { return _headDirection; } }

		public SnakeModel ContinueMove(int s)
		{
			var head = _nodes[0];
			var pt = Point.FromPolar(_headDirection, s);
			if (_nodes.Length == 1)
			{
				return new SnakeModel()
				{
					_nodes = new[] { head.Add(pt), head },
					_length = _length + s,
					_headDirection = _headDirection
				};
			}
			else if (_nodes.Length > 1)
			{
				var nodes = _nodes.ToArray();
				nodes[0] = head.Add(pt);
				return new SnakeModel()
				{
					_nodes = nodes,
					_length = _length + s,
					_headDirection = _headDirection
				};
			}

			throw new Exception();
		}

		public SnakeModel Turn(double alpha)
		{
			var nodes = _nodes;
			if (_nodes.Length > 1)
			{
				var head = _nodes[0];
				nodes = new[] { head }.Concat(_nodes).ToArray();
			}

			return new SnakeModel()
			{
				_nodes = nodes,
				_length = _length,
				_headDirection = _headDirection + alpha
			};

		}

		public SnakeModel Increase(int s)
		{
			if (_nodes.Length == 1)
			{
				var head = _nodes[0];
				var pt = Point.FromPolar(_headDirection, -s);
				return new SnakeModel
				{
					_nodes = new[] { head, pt },
					_length = s,
					_headDirection = _headDirection
				};
			}
			else if (_nodes.Length > 1)
			{
				var nodes = _nodes;
				var tail = new Segment(nodes[nodes.Length - 1], nodes[nodes.Length - 2]);
				var pt = tail.MoveFromAToB(-s);
				nodes[nodes.Length - 1] = pt;
				return new SnakeModel
				{
					_nodes = nodes,
					_length = _length + s,
					_headDirection = _headDirection
				};
			}

			throw new NotImplementedException();
		}

		public SnakeModel Decrease(int s)
		{
			if (_length < s) throw new ArgumentException();
			if (Math.Abs(_length - s) < EPS)
				return new SnakeModel
				{
					_nodes = new[] { _nodes[0] },
					_length = 0,
					_headDirection = _headDirection
				};

			double c;
			int i;
			Segment seg = null;
			for (c = 0, i = 1; i < _nodes.Length && c < _length - s; i++)
			{
				seg = new Segment(_nodes[i], _nodes[i - 1]);
				c += seg.Length;
			}

			var extra = c - (_length - s);
			var nodes = _nodes.Take(i).ToArray();
			if (Math.Abs(extra) > EPS)
			{
				var pt = seg.MoveFromAToB(extra);
				nodes[nodes.Length - 1] = pt;
			}

			return new SnakeModel
			{
				_nodes = nodes,
				_length = c,
				_headDirection = _headDirection
			};
		}
	}
}
