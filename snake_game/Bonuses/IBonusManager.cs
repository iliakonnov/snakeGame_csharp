using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    public interface IBonusManager
    {
        string Name { get; }
        void LoadContent(GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size);
        void Draw(SpriteBatch sb);
    }
}