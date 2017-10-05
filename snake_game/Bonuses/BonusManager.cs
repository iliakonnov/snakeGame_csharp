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
        readonly Dictionary<string, IBonus> _bonuses;

        public BonusManager(Dictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins, MainGame.MainGame game, Random rnd)
        {
            foreach (var plugin in plugins)
            {
                if (config[plugin.Key].IsEnabled)
                {
                    _bonuses[plugin.Key] = plugin.Value.GetBonus(config[plugin.Key], rnd, game);
                }
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (var bonus in _bonuses.Values)
            {
                bonus.LoadContent(graphicsDevice);
            }
        }

        public void Update(GameTime gameTime, int fullTime, CircleF[] snakePoints, Rectangle size)
        {
            foreach (var bonus in _bonuses.Values)
            {
                bonus.Update(gameTime, fullTime, _bonuses, snakePoints, size);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var bonus in _bonuses.Values)
            {
                bonus.Draw(sb);
            }
        }
    }
}