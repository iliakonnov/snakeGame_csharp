using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.Utils;
using Point = snake_game.Utils.Point;
using Void = snake_game.Utils.Void;

namespace AntiAppleBonus
{
    /// <inheritdoc />
    public class Bonus : BonusBase
    {
        private readonly Random _random;

        /// <summary>
        ///     Настройки плагина
        /// </summary>
        public readonly Config Config;

        private bool _created;

        private StarFactory _hexFactory;

        /// <summary>
        ///     Шестигранник бонуса
        /// </summary>
        public Star Hex;

        /// <inheritdoc />
        public Bonus(Config config, Random rnd)
        {
            Config = config;
            _random = rnd;
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch sb)
        {
            if (_created) Hex.PrettyDraw(sb, Config.Color, Config.Thickness);
        }

        /// <inheritdoc />
        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return !plugins.ContainsKey("Snake")
                ? new[] {"Snake"}
                : new string[] { };
        }

        /// <inheritdoc />
        public override void LoadContent(GraphicsDevice gd)
        {
            _hexFactory = new StarFactory(6, 1, Config.Size, gd);
            Hex = _hexFactory.GetStar(new Point(100, 100));
        }

        /// <inheritdoc />
        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles").ToArray();

            if (_created)
            {
                if (Hex.Intersects(snakePoints.First()))
                {
                    plugins["Snake"].GetMethodResult<Void>("Decrease", new object[] {snakePoints.Length / 2});
                    _created = false;
                }
            }
            else
            {
                if (snakePoints.Length > Config.StartSnakeLength &&
                    fullTime % Config.ChanceTime == 0 &&
                    _random.NextDouble() <= Config.NewChance
                )
                {
                    do
                    {
                        Hex = _hexFactory.GetStar(new Point(
                            _random.Next(size.X, size.X + size.Width),
                            _random.Next(size.Y, size.Y + size.Height)
                        ));
                    } while (Hex.Intersects(snakePoints.First()));

                    _created = true;
                }
            }

            return null;
        }
    }
}