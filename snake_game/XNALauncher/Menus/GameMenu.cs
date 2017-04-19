using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game.XNALauncher.Menus
{
    public class GameMenu : IMiniGame
    {
        MainGame.Config.GameConfigClass _config;
        public GameMenu(MainGame.Config.GameConfigClass config)
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
