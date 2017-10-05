using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using Newtonsoft.Json;

namespace snake_game.Bonuses
{
    public class AntiAppleManager : IBonusManager
    {   
        readonly MainGame.Config.BonusConfigClass.AntiAppleConfigClass _config;
        private Polygon _hex;
        public string Name => "antiapple";
        private bool _created;
        private readonly MainGame.MainGame _game;
        private Random _random;

        public void Draw(SpriteBatch sb)
        {
            if (_created)
            {
                _hex.PrettyDraw(sb, _config.Color, _config.Thickness);
            }
        }

        public AntiAppleManager(MainGame.Config.BonusConfigClass.AntiAppleConfigClass config, Random rnd, MainGame.MainGame game)
        {
            _game = game;
            _config = config;
            _random = rnd;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _hex = new Polygon(6, _config.Size, new Vector2(100, 100));
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
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
