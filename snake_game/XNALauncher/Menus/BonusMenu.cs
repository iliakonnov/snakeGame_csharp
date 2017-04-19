using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game.XNALauncher.Menus
{
    public class BonusMenu : IMiniGame
    {
        MainGame.Config.BonusConfigClass _config;
        public BonusMenu(MainGame.Config.BonusConfigClass config)
        {
            _config = config;
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameWindow window)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime, GameWindow window)
        {
            throw new NotImplementedException();
        }
    }
}
