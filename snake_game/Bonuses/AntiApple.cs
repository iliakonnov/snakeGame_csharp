using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    class AntiAppleManager : IBonusManager
    {
        public string Name => "antiapple";

        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
        {
            throw new NotImplementedException();
        }
    }
}
