using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
    public class BonusManager
    {
        public readonly Dictionary<string, BonusBase> Bonuses;

        private Dictionary<string, Accessable> _currentEvents =
            new Dictionary<string, Accessable>
            {
                {"_game", null}
            };

        public BonusManager(IReadOnlyDictionary<string, IPluginConfig> config, Dictionary<string, IPlugin> plugins,
            MainGame.MainGame game, Random rnd)
        {
            Bonuses = plugins
                .Where(x => config[x.Key].IsEnabled)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.GetBonus(config[x.Key], rnd, game)
                );
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            Dictionary<string, List<string>> notLoaded;
            do
            {
                notLoaded = new Dictionary<string, List<string>>();
                foreach (var kv in Bonuses)
                foreach (var dependency in kv.Value.CheckDependincies(Bonuses))
                {
                    if (notLoaded.ContainsKey(kv.Key))
                        notLoaded[kv.Key].Add(dependency);
                    else
                        notLoaded[kv.Key] = new List<string> {dependency};
                }
                foreach (var kv in notLoaded)
                {
                    var message = new StringBuilder($"Plugin '{kv.Key}' cannot be loaded because following plugins are not loaded: ");
                    foreach (var name in kv.Value)
                    {
                        message.Append($"'{name}' ");
                    }
                    // TODO: Show message
                    Bonuses.Remove(kv.Key);
                }
            } while (notLoaded.Count != 0);  // Если плагин не был загружен, то заново проверяет зависимости у всех
            
            foreach (var bonus in Bonuses.Values)
            {
                bonus.LoadContent(graphicsDevice);
            }
        }

        public void Update(GameTime gameTime, int fullTime, KeyboardState keyboardState, Rectangle size, Accessable gameEvents)
        {
            var nextEvents = new Dictionary<string, Accessable>
            {
                {"_game", gameEvents}
            };
            foreach (var kvPair in Bonuses)
            {
                nextEvents[kvPair.Key] =
                    kvPair.Value.Update(gameTime, fullTime, keyboardState, Bonuses, size, _currentEvents);
            }
            _currentEvents = nextEvents;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var bonus in Bonuses.Values)
            {
                bonus.Draw(sb);
            }
        }
    }
}