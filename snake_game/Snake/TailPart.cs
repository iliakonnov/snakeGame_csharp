using Microsoft.Xna.Framework;

namespace snake_game.Snake
{
	public enum TailType
	{
		Tail, Head
	}

	public class TailPart
	{
		public double RotateRadians;
		public TailType Type;
		public Rectangle CollisionBox;
		public Vector2 Position;
		public Vector2 Direction;
	}
	public class Turn
	{
		public Vector2 Position;
		public double OldRadians;
		public double NewRadians;
	}
}
