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
        private readonly Dictionary<string, BonusBase> _bonuses;

        private Dictionary<string, IReadOnlyDictionary<string, Gettable>> _currentEvents =
            new Dictionary<string, IReadOnlyDictionary<string, Gettable>>
            {
                {"_game", null}
            };

        public BonusManager(IReadOnlyDictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins,
            MainGame.MainGame game, Random rnd)
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
            foreach (var bonus in _bonuses.Values)
            {
                bonus.LoadContent(graphicsDevice);
            }
        }

        public void Update(GameTime gameTime, int fullTime, CircleF[] snakePoints, Rectangle size,
            [NullGuard.AllowNull] IReadOnlyDictionary<string, Gettable> gameEvents)
        {
            var nextEvents = new Dictionary<string, IReadOnlyDictionary<string, Gettable>>
            {
                {"_game", gameEvents}
            };
            foreach (var kvPair in _bonuses)
            {
                nextEvents[kvPair.Key] =
                    kvPair.Value.Update(gameTime, fullTime, _bonuses, snakePoints, size, _currentEvents);
            }
            _currentEvents = nextEvents;
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