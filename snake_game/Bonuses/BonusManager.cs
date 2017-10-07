using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
    public class BonusManager
    {
        readonly Dictionary<string, IBonus> _bonuses = new Dictionary<string, IBonus>();

        public BonusManager(Dictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins, MainGame.MainGame game, Random rnd)
        {
            _bonuses = plugins
                .Where(x => config[x.Key].IsEnabled)
                .ToDictionary(
                    x => x.Key, 
                    x => x.Value.GetBonus(config[x.Key], rnd, game)
                );
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            // Вот тут вот нужно передавать бонусам всё необходимое для компиляции функций
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