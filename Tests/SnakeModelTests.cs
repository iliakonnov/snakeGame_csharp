using snake_game.Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class SnakeModelTests
	{

		[Fact]
		public void ContinueMoveTest_1()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0),
				new Point(3, 1)
			};
			var snake = new SnakeModel(135, snakePoints);

			var diag = new Segment(snakePoints[1], snakePoints[2]).Length;
			var newSnake = snake.ContinueMove(diag);

			var newSnakePoints = new[] {
				new Point(-1, 1),
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0)
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(135, newSnake.Direction);
			Assert.Equal(snake.Length, newSnake.Length);
		}
		[Fact]
		public void ContinueMoveTest_2()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 0),
				new Point(1, 1),
				new Point(3, 1),
				new Point(4, 1),
			};
			var snake = new SnakeModel(90, snakePoints);

			var diag = 0.85f;
			var newSnake = snake.ContinueMove(diag);

			var newSnakePoints = new[] {
				new Point(0, 0.85f),
				new Point(0, 0),
				new Point(1, 1),
				new Point(3, 1),
				new Point(3.15f, 1),
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(90, newSnake.Direction);
			Assert.Equal(snake.Length, newSnake.Length);
		}

		[Fact]
		public void TurnTest()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0),
				new Point(3, 1)
			};
			var snake = new SnakeModel(135, snakePoints);

			var newSnake = snake.Turn(15);

			var newSnakePoints = new[] {
				new Point(0, 0),
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0),
				new Point(3, 1)
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(135 + 15, newSnake.Direction);
			Assert.Equal(snake.Length, newSnake.Length);
		}

		[Fact]
		public void TurnTest_2()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0),
				new Point(3, 1)
			};
			var snake = new SnakeModel(135, snakePoints);

			var newSnake = snake.Turn(15);

			var newSnakePoints = new[] {
				new Point(0, 0),
				new Point(0, 0),
				new Point(1, 1),
				new Point(2, 0),
				new Point(3, 1)
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(135 + 15, newSnake.Direction);
			Assert.Equal(snake.Length, newSnake.Length);
		}

		[Fact]
		public void IncreaseTest()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 1),
				new Point(1, 1),
				new Point(2, 1)
			};
			var snake = new SnakeModel(135, snakePoints);

			var step = 1.395f;
			var newSnake = snake.Increase(step);

			var newSnakePoints = new[] {
				new Point(0, 0),
				new Point(0, 1),
				new Point(1, 1),
				new Point(2+step, 1)
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(snake.Length + step, newSnake.Length);
		}

		[Fact]
		public void DecreaseTest()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 1),
				new Point(1, 1),
				new Point(3, 1)
			};
			var snake = new SnakeModel(135, snakePoints);

			var step = 1.723f;
			var newSnake = snake.Decrease(step);

			var newSnakePoints = new[] {
				new Point(0, 0),
				new Point(0, 1),
				new Point(1, 1),
				new Point(3-step, 1)
			};

			Assert.Equal(newSnakePoints, newSnake.Points.ToArray());
			Assert.Equal(snake.Length - step, newSnake.Length);
		}

		[Fact]
		public void GetSnakeAsPointsTests()
		{
			var snakePoints = new[] {
				new Point(0, 0),
				new Point(0, 4),
				new Point(4, 4),
				new Point(1, 1),
				new Point(1, 3),
			};

			var snake = new SnakeModel(0, snakePoints);

			var dist = 1.7f;

			var a = (float)(dist / Math.Sqrt(2));
			var x = (float)((5 * dist - 8) / Math.Sqrt(2));
			var snakeCircles = new[] {
				new Point(0, 0),
				new Point(0, dist),
				new Point(0, 2*dist),
				new Point(dist*3-4, 4),
				new Point(dist*4-4, 4),
				new Point(4-x, 4-x),
				new Point(4-x-a, 4-x-a),
				new Point(4-x-2*a, 4-x-2*a),
				new Point(1, 2.35736f),
			};

			var answer = snake.GetSnakeAsPoints(dist);
			Assert.Equal(snakeCircles.Length, answer.Length);
			for (int i = 0; i < snakeCircles.Length; i++)
			{
				Assert.Equal(snakeCircles[i], answer[i]);
			}

			Assert.Equal(snakeCircles, answer);

		}
	}
}
