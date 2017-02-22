using snake_game.Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
	public class SegmentTests
	{
		[Fact]
		public void AsSetOfPointsTest()
		{
			var seg = new Segment
			(
				new Point(0, 0),
				new Point(5, 0)
			);

			var tuple = seg.AsSetOfPoints(1.3f, 0.8f);

			var expected = new[] {
					new Point(0.8f, 0),
					new Point(2.1f, 0),
					new Point(3.4f, 0),
					new Point(4.7f, 0),
				};

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.Equal(expected[i], tuple.Item1[i]);
			}

			Assert.Equal(expected, tuple.Item1);

			Assert.True(1e-5 > Math.Abs(tuple.Item2 - 1));

		}

		[Fact]
		public void AsSetOfPointsTest_zero()
		{
			var seg = new Segment
			(
				new Point(0, 0),
				new Point(5, 0)
			);

			var tuple = seg.AsSetOfPoints(1.3f, 0);

			var expected = new[] {
					new Point(1.3f, 0),
					new Point(2.6f, 0),
					new Point(3.9f, 0),
				};

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.Equal(expected[i], tuple.Item1[i]);
			}

			Assert.Equal(expected, tuple.Item1);

			Assert.True(1e-5 > Math.Abs(tuple.Item2 - 0.2));

		}

		[Fact]
		public void AsSetOfPointsTest_end()
		{
			var seg = new Segment
			(
				new Point(0, 0),
				new Point(5, 0)
			);

			var tuple = seg.AsSetOfPoints(1.3f, 1.1f);

			var expected = new[] {
					new Point(1.1f, 0),
					new Point(2.4f, 0),
					new Point(3.7f, 0),
					new Point(5.0f, 0),
				};

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.Equal(expected[i], tuple.Item1[i]);
			}

			Assert.Equal(expected, tuple.Item1);

			Assert.True(1e-5 > Math.Abs(tuple.Item2 - 1.3));
			//Assert.Equal(1e-5, tuple.Item2);

		}
	}
}
