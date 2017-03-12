using snake_game.Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Snake
{
	public class MathUtilsTests
	{
		[Fact]
		public void PointSegmentDistance_1()
		{
			var seg = new Segment(new Point(0, 0), new Point(3, 0));
			var pt = new Point(2, 1);

			var dist = MathUtils.Distance(seg, pt);
			Assert.Equal(1f, dist);
		}
		[Fact]
		public void PointSegmentDistance_2()
		{
			var seg = new Segment(new Point(0, 0), new Point(3, 0));
			var pt = new Point(6, 4);

			var dist = MathUtils.Distance(seg, pt);
			Assert.Equal(5f, dist);
		}
	}
}
