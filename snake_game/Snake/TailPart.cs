using Microsoft.Xna.Framework;

namespace snake_game.Snake
{
	public enum TailType
	{
		tail, head
	}

	public class TailPart
	{
		public double rotateRadians;
		public TailType type;
		public Rectangle collisionBox;
		public Vector2 position;
		public Vector2 direction;
	}
	public class Turn
	{
		public Vector2 position;
		public double oldRadians;
		public double newRadians;
	}
}
