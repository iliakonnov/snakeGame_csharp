using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using snake_game;
using snake_game.Bonuses;
using snake_game.Utils;
using Polygon = snake_game.Utils.Polygon;
using Void = snake_game.Utils.Void;

namespace AntiAppleBonus
{
    public class Bonus : BonusBase
    {
        readonly Config _config;
        private Polygon _hex;
        private bool _created;
        private readonly snake_game.MainGame.MainGame _game;
        private Random _random;

        public override void Draw(SpriteBatch sb)
        {
            if (_created)
            {
                _hex.PrettyDraw(sb, _config.Color, _config.Thickness);
            }
        }

        public Bonus(Config config, Random rnd, snake_game.MainGame.MainGame game)
        {
            _game = game;
            _config = config;
            _random = rnd;
        }

        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            return !plugins.ContainsKey("Snake")
                ? new[] {"Snake"}
                : new string[] { };
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _hex = new Polygon(6, _config.Size, new Vector2(100, 100));
        }

        public override Accessable Update(GameTime gameTime, int fullTime, KeyboardState keyboardState,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles").ToArray();

            if (_created)
            {
                if (_hex.Intersects(snakePoints.First()))
                {
                    plugins["Snake"].GetMethodResult<Void>("Decrease", new object[] {snakePoints.Length / 2});
                    _created = false;
                }
            }
            else
            {
                if (snakePoints.Length > _config.StartSnakeLength &&
                    fullTime % _config.ChanceTime == 0 &&
                    _random.NextDouble() <= _config.NewChance
                )
                {
                    do
                    {
                        _hex = new Polygon(6, _config.Size, new Vector2(
                            _random.Next(size.X, size.X + size.Width),
                            _random.Next(size.Y, size.Y + size.Height)
                        ));
                    } while (_hex.Intersects(snakePoints.First()));
                    _created = true;
                }
            }

            return null;
        }
    }
}