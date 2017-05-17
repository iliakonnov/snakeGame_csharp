using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

/*
 * Для добавления бонусов нужно:
 * 1. Сделать сам бонус. Это должен быть класс, реализующий snake_game.Bonuses.IBonusManager
 * 2. Зарегистрировать бонус в этом файле. Для этого его нужно добавить в switch на строке №39 и в AvailableBonuses на строке № 20
 * 3. Сделать класс настроек и добавить его в MainGame.Config.BonusConfigClass
 * 4. Сделать окно настройки. Это должен быть класс, рализующий snake_game.Launcher.Bonuses.IBonusConfig, где T -- класс настроек из пункта 3
 * 5. Зарегистрировать бонус в snake_game.Launcher.LauncherForm в Draw()/Content/Items/TabControl[0]/Pages на строке №101
 */
namespace snake_game.Bonuses
{
    public class BonusManager
    {
        public static readonly string[] AvailableBonuses = {"brick", "apple", "antibrick", "antiapple"};
        Config.BonusConfigClass _config;
        Random _rnd;
        readonly IBonusManager[] _bonuses;

        public BonusManager(Config.BonusConfigClass config, MainGame.MainGame game, Random rnd)
        {
            if (config.BonusSettings.EnableBonuses)
            {
                _config = config;
                _rnd = rnd;
                var bonuses = new List<IBonusManager>();
                var bonusesEnabled =
                    config.BonusSettings.BonusesEnabled == null ||
                    config.BonusSettings.BonusesEnabled.Length == 0
                        ? AvailableBonuses
                        : config.BonusSettings.BonusesEnabled;
                foreach (var bonus in bonusesEnabled)
                {
                    switch (bonus)
                    {
                        case "brick":
                            bonuses.Add(new BrickManager(_config.BrickConfig, _rnd, game));
                            break;
                        case "apple":
                            bonuses.Add(new AppleManager(_config.AppleConfig, _rnd, game));
                            break;
                        case "antibrick":
                            bonuses.Add(new AntiBrickManager(_config.AntiBrickConfig, _rnd, game));
                            break;
                        case "antiapple":
                            // bonuses.Add(new AntiAppleManager(_config.AntiBrickConfig, _rnd, game));
                            break;
                        default:
                            throw new ArgumentException($"Unknown bonus: {bonus}");
                    }
                }
                _bonuses = bonuses.ToArray();
            }
            else
            {
                _bonuses = new IBonusManager[] { };
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.LoadContent(graphicsDevice);
            }
        }

        public void Update(GameTime gameTime, int fullTime, CircleF[] snakePoints, Rectangle size)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.Update(gameTime, fullTime, _bonuses, snakePoints, size);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.Draw(sb);
            }
        }
    }
}