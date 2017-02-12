using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using snake_game.Snake;
using SnakePoint = snake_game.Snake.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace snake_game.MainGame
{
    public class LineDrawer
    {
        readonly Texture2D _t;
        readonly int _width;
        readonly Color _color;

        public LineDrawer(int width, Color clr, GraphicsDevice gd)
        {
            _t = new Texture2D(gd, 1, 1);
            _t.SetData(new[] {clr});

            _color = clr;
            _width = width;
        }

        /// <summary>
        /// Draws line
        /// </summary>
        /// <remarks>
        /// http://gamedev.stackexchange.com/a/44016
        /// </remarks>
        /// <param name="sb">Sprite batch</param>
        /// <param name="seg">Segment</param>
        public void DrawLine(SpriteBatch sb, Segment seg)
        {
			InternalDrawLine(sb, seg);
			var vp = sb.GraphicsDevice.Viewport;
			if (((int)seg.A.X == vp.Width && (int)seg.B.X == vp.Width) ||
			    ((int)seg.A.Y == vp.Height && (int)seg.B.Y == vp.Height) ||
			    ((int)seg.A.X == 0 && (int)seg.B.X == 0) ||
				((int)seg.A.Y == 0 && (int)seg.B.Y == 0)
			   )
				// Draw reverse
				InternalDrawLine(sb, new Segment(seg.B, seg.A));
        }

		void InternalDrawLine(SpriteBatch sb, Segment seg)
		{
			var start = new Vector2(seg.A.X, seg.A.Y);
			var end = new Vector2(seg.B.X, seg.B.Y);

			// ReSharper disable once SuggestVarOrType_SimpleTypes
			Vector2 edge = end - start;
			// calculate angle to rotate line
			var angle = (float)Math.Atan2(edge.Y, edge.X);

			var line = new Rectangle( // rectangle defines shape of line and position of start of line
				(int)start.X,
				(int)start.Y,
				(int)edge.Length(), //sb will strech the texture to fill this rectangle
				 _width //width of line, change this to make thicker line
			);

			sb.Draw(
				_t,
				line,
				null,
				_color, //colour of line
				angle, //angle of line (calulated above)
				Vector2.Zero, // point in line about which to rotate
				SpriteEffects.None,
				0
			);
		}

		// Overloads

		/// <summary>
		/// Draws line between two points
		/// </summary>
		/// <param name="sb">Sprite batch</param>
		/// <param name="a">Start point</param>
		/// <param name="b">End point</param>
		public void DrawLine(SpriteBatch sb, SnakePoint a, SnakePoint b)
        {
            DrawLine(sb, new Segment(a, b));
        }

        /// <summary>
        /// Draws line between two points
        /// </summary>
        /// <param name="sb">Sprite batch</param>
        /// <param name="a">Start point</param>
        /// <param name="b">End point</param>
        public void DrawLine(SpriteBatch sb, XnaPoint a, XnaPoint b)
        {
            DrawLine(sb, new Segment(
                new Snake.Point(a.X, a.Y),
                new Snake.Point(b.X, b.Y)
            ));
        }

        /// <summary>
        /// Draws line between two points
        /// </summary>
        /// <param name="sb">Sprite batch</param>
        /// <param name="a">Start point</param>
        /// <param name="b">End point</param>
        public void DrawLine(SpriteBatch sb, Vector2 a, Vector2 b)
        {
            DrawLine(sb, new Segment(
                new Snake.Point(a.X, a.Y),
                new Snake.Point(b.X, b.Y)
            ));
        }
    }
}