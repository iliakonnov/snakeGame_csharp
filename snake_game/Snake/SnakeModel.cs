	using System;
using System.Linq;
using System.Collections.Generic;

namespace snake_game
{
	public struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }
	}

	public interface IWorld
	{
		int Distance(Point a, Point b, int direction);
		Point Normalize(Point a);
	}

	public class SnakeModel
	{
		class Node
		{
			public Node(Point pt, int turn)
			{
				X = pt.X;
				Y = pt.Y;
				Turn = turn;
			}
			public int X { get; private set; }
			public int Y { get; private set; }
			public Point Point { get { return new Point { X = X, Y = Y }; } }
			public Point Move(int s)
			{
				var newX = Convert.ToInt32(X + s * Math.Cos(Turn * Math.PI / 180));
				var newY = Convert.ToInt32(Y + s * Math.Sin(Turn * Math.PI / 180));
				return new Point { X = newX, Y = newY };
			}
			public int Turn { get; private set; }
		}

		List<Node> _nodes = new List<Node>();
		int _length;
		IWorld _world;

		private SnakeModel() { }
		public SnakeModel(IWorld w, Point head, int turn)
		{
			_world = w;
			_nodes.Add(new Node(head, turn));
		}

		public SnakeModel ContinueMove(int s)
		{
			var newNodes = _nodes.ToList();
			var head = newNodes[0];
			var newHeadPoint = _world.Normalize(head.Move(s));

			newNodes[0] = new Node(newHeadPoint, head.Turn);
			if (_nodes.Count == 1)
			{
				return new SnakeModel { _nodes = newNodes, _length = 0, _world = _world };
			}

			return CutTail(newNodes, _length);
		}

		public SnakeModel Turn(int turn)
		{
			var newNodes = _nodes.ToList();
			var head = newNodes[0];
			turn = (head.Turn + turn)%360;
			if (turn < 0) turn += 360;
			newNodes[0] = new Node(head.Point, turn);
			return new SnakeModel { _nodes = newNodes, _length = _length, _world = _world };
		}

		public SnakeModel IncreaseLength(int s)
		{
			var newNodes = _nodes.ToList();
			Node newTail = null;

			if (_nodes.Count == 1)
			{
				var head = newNodes[0];
				newTail = new Node(_world.Normalize(head.Move(-s)), head.Turn);
				newNodes.Add(newTail);
				return new SnakeModel { _nodes = newNodes, _length = s, _world = _world };
			}

			var tail = newNodes.Last();
			newTail = new Node(_world.Normalize(tail.Move(-s)), tail.Turn);
			newNodes[newNodes.Count-1] = newTail;
			return new SnakeModel { _nodes = newNodes, _length = _length + s, _world = _world };
		}

		public SnakeModel DecreaseLength(int s)
		{
			if (_length < s) throw new ArgumentException();
			if (_length == s)
			{ 
				return new SnakeModel { _nodes = new List<Node> { _nodes.First()} , _length = 0, _world = _world };
			}

			var newNodes = _nodes.ToList();
			return CutTail(newNodes, _length - s);
		}

		private SnakeModel CutTail(List<Node> newNodes, int length)
		{ 
			int newLength = 0;
			int i;
			for (i = 1; i < newNodes.Count && newLength < _length; i++)
			{
				newLength += _world.Distance(
					newNodes[i].Point,
					newNodes[i - 1].Point,
					newNodes[i].Turn);
			}

			if (newLength == _length)
			{
				newNodes = newNodes.Take(i + 1).ToList();
				return new SnakeModel { _nodes = newNodes, _length = _length, _world = _world };
			}

			if (newLength > _length)
			{
				newNodes = newNodes.Take(i + 1).ToList();
				var extraLength = newLength - _length;
				var tail = newNodes.Last();
				var newTailPoint = _world.Normalize(tail.Move(extraLength));
				newNodes[i] = new Node(newTailPoint, tail.Turn);
				return new SnakeModel { _nodes = newNodes, _length = _length, _world = _world };
			}

			throw new Exception();
		}


		public bool IsSelfIntersected
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsIntersected(Point pt, int width, int height)
		{
			throw new NotImplementedException();
		}
	}
}
