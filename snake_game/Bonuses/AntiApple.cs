using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    class AntiAppleManager : IBonusManager
    {
        readonly MainGame.Config.BonusConfigClass.AntiAppleConfigClass _config;
        PolygonF _hex;
        public string Name => "antiapple";

        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();
        }

        public AntiAppleManager(MainGame.Config.BonusConfigClass.AntiAppleConfigClass config)
        {
            _config = config;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            var halfSqrt3 = Math.Sqrt(3) / 2;
            _hex = new PolygonF(new Vector2[] {
                new Vector2(1 * _config.Size, 0),
                new Vector2((float)(0.5 * _config.Size), (float)(halfSqrt3 * _config.Size)),
                new Vector2((float)(-0.5  * _config.Size), (float)(halfSqrt3 * _config.Size)),
                new Vector2(-1 * _config.Size, 0),
                new Vector2((float)(-0.5 * _config.Size), (float)(-halfSqrt3 * _config.Size)),
                new Vector2((float)(0.5 * _config.Size), (float)(-halfSqrt3 * _config.Size))
            });
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
        {
            throw new NotImplementedException();
        }
    }
}
