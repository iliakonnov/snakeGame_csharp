using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game;
using snake_game.Bonuses;

namespace AntiAppleBonus
{
    public class Bonus : IBonus
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

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _hex = new Polygon(6, _config.Size, new Vector2(100, 100));
        }

        public override void Update(GameTime gameTime, int fullTime, Dictionary<string, IBonus> plugins, CircleF[] snakePoints, Rectangle size)
        {
            if (_created)
            {
                if (_hex.Intersects(snakePoints.First()))
                {
                    _game.Slim(snakePoints.Length / 2);
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
        }
    }
}
