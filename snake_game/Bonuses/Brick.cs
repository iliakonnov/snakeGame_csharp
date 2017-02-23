using snake_game.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
	class Brick : BonusBase
	{
		List<BrickModel> _bricks;
		Config.BonusConfigClass.BrickConfigClass _config;
		Texture2D Texture;

		static Brick()
		{
			BonusFactory.RegisterBonus(nameof(Brick), typeof(Brick));
		}

		public override void Draw(SpriteBatch sb)
		{
			foreach (var item in _bricks)
			{
				item.Draw(sb);
			}
		}
		public override void Init(Config.BonusConfigClass config)
		{
			_config = config.BrickConfig;
		}

		public override void LoadContent(GraphicsDevice gd)
		{
			Texture = new Texture2D(gd, _config.size, _config.size);
		}

		public override void Update(GameTime time)
		{
			foreach (var item in _bricks)
			{
				item.Update(time);
			}
		}

		public override bool Collides(IShapeF shape)
		{
			var bound = shape.GetBoundingRectangle();
			return Collides(bound);
		}
		public bool Collides(RectangleF shape)
		{
			foreach (var item in _bricks)
			{
				if (item.Collides(shape))
				{
					return true;
				}
			}
			return false;
		}
		public bool Collides(CircleF shape)
		{
			foreach (var item in _bricks)
			{
				if (item.Collides(shape))
				{
					return true;
				}
			}
			return false;
		}
	}
	public class BrickModel : BonusModel
	{
		Vector2 _position;
		int _size;
		BrickModel(Brick bonus, Vector2 position, int size) : base(bonus)
		{
			_position = position;
			_size = size;
		}
		public override void Draw(SpriteBatch sb)
		{
			throw new NotImplementedException();
		}

		public override void Update(GameTime time)
		{
			throw new NotImplementedException();
		}

		public override bool Collides(IShapeF shape)
		{
			var bound = shape.GetBoundingRectangle();
			return Collides(bound);
		}
		public bool Collides(RectangleF shape)
		{
			throw new NotImplementedException();
		}
		public bool Collides(CircleF shape)
		{
			throw new NotImplementedException();
		}
	}
}
