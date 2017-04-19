using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game
{
    public interface IMiniGame
    {
        void LoadContent(GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime, GameWindow window);
        void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameWindow window);
    }
}
