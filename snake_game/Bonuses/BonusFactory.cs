using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snake_game.Bonuses
{
	static class BonusFactory
	{
		static Dictionary<string, BonusBase> _bonuses = new Dictionary<string, BonusBase>();
		public static void RegisterBonus(string name, Type bonusType)
		{
			if (!bonusType.IsAssignableFrom(typeof(BonusBase)))
				throw new ArgumentException();

			_bonuses.Add(name, (BonusBase)Activator.CreateInstance(bonusType));
		}

		public static Dictionary<string, BonusBase> RegisteredBonuses
		{
			get { return _bonuses; }
		}
	}
}
