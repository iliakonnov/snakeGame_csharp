using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game.Bonuses
{
    /// <summary>
    ///     Управляет всеми загруженными бонусами
    /// </summary>
    public class BonusManager
    {
        /// <summary>
        ///     Словарь всех загруженных бонусов
        /// </summary>
        public readonly Dictionary<string, BonusBase> Bonuses;

        private Dictionary<string, Accessable> _currentEvents =
            new Dictionary<string, Accessable>
            {
                {"_game", null}
            };

        /// <inheritdoc />
        /// <param name="config">Параметры всех бонусов</param>
        /// <param name="plugins">Все бонусы</param>
        /// <param name="game">Объект ядра игры</param>
        /// <param name="rnd">Источник случайных чисел</param>
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

        /// <summary>
        ///     Вызывает <see cref="BonusBase.LoadContent(GraphicsDevice)" /> у всех бонусов.
        ///     Соответствует <see cref="Game.LoadContent()" />.
        /// </summary>
        /// <param name="graphicsDevice">См. <see cref="Game.GraphicsDevice" /></param>
        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            Dictionary<string, List<string>> notLoaded;
            do
            {
                notLoaded = new Dictionary<string, List<string>>();
                foreach (var kv in Bonuses)
                foreach (var dependency in kv.Value.CheckDependincies(Bonuses))
                    if (notLoaded.ContainsKey(kv.Key))
                        notLoaded[kv.Key].Add(dependency);
                    else
                        notLoaded[kv.Key] = new List<string> {dependency};

                foreach (var kv in notLoaded)
                {
                    var message =
                        new StringBuilder(
                            $"Plugin '{kv.Key}' cannot be loaded because following plugins are not loaded: ");
                    foreach (var name in kv.Value) message.Append($"'{name}' ");

                    // TODO: Show message
                    Bonuses.Remove(kv.Key);
                }
            } while (notLoaded.Count != 0); // Если плагин не был загружен, то заново проверяет зависимости у всех

            foreach (var bonus in Bonuses.Values) bonus.LoadContent(graphicsDevice);
        }

        /// <summary>
        ///     Вызывает
        ///     <see
        ///         cref="BonusBase.Update(GameTime,int,KeyboardState,System.Collections.Generic.IReadOnlyDictionary{string,snake_game.Bonuses.BonusBase},Rectangle,System.Collections.Generic.IReadOnlyDictionary{string,snake_game.Bonuses.Accessable})" />
        ///     у всех бонусов.
        ///     Соответствует <see cref="Game.Update(GameTime)" />.
        /// </summary>
        /// <param name="gameTime">См. параметр <c>gameTime</c> в <see cref="Game.Update(GameTime)" /></param>
        /// <param name="fullTime">Суммарное время игры в миллисекундах</param>
        /// <param name="keyboardState">Состояние клавиатуры в данный тик</param>
        /// <param name="size">Размер экрана</param>
        /// <param name="gameEvents">События игры за текущий тик</param>
        public void Update(GameTime gameTime, int fullTime, KeyboardState keyboardState, Rectangle size,
            Accessable gameEvents)
        {
            var nextEvents = new Dictionary<string, Accessable>
            {
                {"_game", gameEvents}
            };
            foreach (var kvPair in Bonuses)
                nextEvents[kvPair.Key] =
                    kvPair.Value.Update(gameTime, fullTime, keyboardState, Bonuses, size, _currentEvents);

            _currentEvents = nextEvents;
        }

        /// <summary>
        ///     Вызывает <see cref="BonusBase.Draw(SpriteBatch)" /> у всех бонусов
        ///     Соответствует <see cref="Game.Draw(GameTime)" />
        /// </summary>
        /// <param name="sb">Уже открытый SpriteBatch</param>
        public void Draw(SpriteBatch sb)
        {
            foreach (var bonus in Bonuses.Values) bonus.Draw(sb);
        }
    }
}