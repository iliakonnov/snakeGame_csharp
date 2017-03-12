using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace snake_game.Snake
{
	/// <summary>
	/// Модель змеи, состоящей из отрезков. Голова может двигаться в любом направлении.
	/// </summary>
	public class SnakeModel
	{
		const float EPS = 1e-6f;

		// Длина змеи
		float _length;
		public float Length => _length;

		// точки, задающие повороты змеи. Первый - голова
		Point[] _nodes;
		// направление движения головы в градусах
		float _headDirection;

		SnakeModel() { }
		public SnakeModel(Point head, int direction)
		{
			_length = 0;
			_nodes = new[] { head };
			_headDirection = direction;
		}

		internal SnakeModel(float direction, Point[] points)
		{
			_nodes = points.ToArray();
			_headDirection = direction;
			_length = 0;
			for (int i = 0; i < points.Length - 1; i++)
			{
				_length += new Segment(points[i], points[i + 1]).Length;
			}
		}

		public IReadOnlyCollection<Point> Points => new ReadOnlyCollection<Point>(_nodes);

		public float Direction => _headDirection;

	    public SnakeModel ContinueMove(float s)
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
					_headDirection = _headDirection
				};
			}

			var nodes = _nodes.ToArray();
			nodes[0] = head.Add(pt);
			return SetLength(_length, nodes, _headDirection);
		}

		public SnakeModel Turn(float alpha)
		{
			if (Math.Abs(alpha) <= EPS) return this;
			var nodes = _nodes;
			if (_nodes.Length > 1)
			{
			    var head = _nodes[0];
			    nodes = head.Equals(_nodes[1])
			        ? _nodes
			        : new[] { head }.Concat(_nodes).ToArray();
			}

			var dir = ((int)(_headDirection + alpha)) % 360;
			if (dir < 0) dir += 360;
			return new SnakeModel
			{
				_nodes = nodes,
				_length = _length,
				_headDirection = dir
			};

		}

		public SnakeModel TurnAt(float degrees)
		{
			return Turn(degrees - Direction);
		}

		public SnakeModel Increase(float s)
		{
			return SetLength(_length + s, _nodes, _headDirection);
		}

		public SnakeModel Decrease(float s)
		{
			if (_length < s) throw new ArgumentException();
			if (Math.Abs(_length - s) < EPS)
				return new SnakeModel
				{
					_nodes = new[] { _nodes[0] },
					_length = 0,
					_headDirection = _headDirection
				};

			return SetLength(_length - s, _nodes, _headDirection);
		}

		public Point[] GetSnakeAsPoints(float pointDistance)
		{
			var result = new Point[(int)(_length / pointDistance)];
			var n = 0;
			result[n++] = _nodes[0];
			var skip = 0f;
			for (var i = 0; i < _nodes.Length - 1; i++)
			{
				Tuple<Point[], float> tuple = new Segment(_nodes[i], _nodes[i + 1]).AsSetOfPoints(pointDistance, skip);
				foreach (var item in tuple.Item1)
				{
					if (n < result.Length) result[n++] = item;
				}
				skip = tuple.Item2;
			}
		    if (result[result.Length - 1] == null)
		    {
		        // TODO: Змея дёргается если расстояние между кружочками кратно 20
		        result[result.Length - 1] = result[result.Length - 2];
		    }

			return result;
		}

		internal static SnakeModel SetLength(float length, Point[] nodes, float headDirection)
		{
			if (nodes.Length == 0) throw new Exception();

			if (nodes.Length == 1)
			{
				var head = nodes[0];
				var pt = Point.FromPolar(headDirection, -length);
				return new SnakeModel
				{
					_nodes = new[] { head, head.Add(pt) },
					_length = length,
					_headDirection = headDirection
				};
			}

			int i;
			float c;
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
					_nodes = newNodes,
					_length = c,
					_headDirection = headDirection
				};
			}
			else
			{
				// надо укоротить или удлинить
				var pt = tail.MoveFromAToB(c - length);
				newNodes[newNodes.Length - 1] = pt;
				return new SnakeModel
				{
					_nodes = newNodes,
					_length = length,
					_headDirection = headDirection
				};
			}
		}
	}
}
