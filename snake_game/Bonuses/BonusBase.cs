using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
	public abstract class BonusBase
	{
		public abstract void Init(Config.BonusConfigClass config);
		public abstract void LoadContent(GraphicsDevice gd);
		public abstract void Update(GameTime time);
		public abstract void Draw(SpriteBatch sb);
		public abstract bool Collides(IShapeF shape);
	}
	public abstract class BonusModel
	{
		protected BonusBase _bonus;
		protected BonusModel(BonusBase bonus) { _bonus = bonus; }
		public abstract void Update(GameTime time);
		public abstract void Draw(SpriteBatch sb);
		public abstract bool Collides(IShapeF shape);
	}
}
