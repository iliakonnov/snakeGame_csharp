using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using snake_game.Bonuses;
using System.Collections.Generic;
using System.Linq;

namespace snake_game.MainGame
{
	class BonusManager
	{
		BonusBase[] _bonuses;
		public BonusManager(Config.BonusConfigClass config)
		{
			// Locate bonuses
			var bonuses = BonusFactory.RegisteredBonuses;
			if (config.BonusSettings.BonusesEnabled == null)
			{
				_bonuses = bonuses.Values.ToArray<BonusBase>();
			}
			else
			{
				var newBonuses = new BonusBase[config.BonusSettings.BonusesEnabled.Length];
				for (int i = 0; i < config.BonusSettings.BonusesEnabled.Length; i++)
				{
					var bonus = bonuses[config.BonusSettings.BonusesEnabled[i]];
					bonus.Init(config);
					newBonuses[i] = bonus;
				}
				_bonuses = newBonuses;
			}
		}

		public void LoadContent(GraphicsDevice gd)
		{
			foreach (var b in _bonuses)
			{
				b.LoadContent(gd);
			}
		}

		public void Update(GameTime time)
		{
			foreach (var b in _bonuses)
			{
				b.Update(time);
			}
		}

		public void Draw(SpriteBatch sb)
		{
			foreach (var b in _bonuses)
			{
				b.Draw(sb);
			}
		}
	}
}
