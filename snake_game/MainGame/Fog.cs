using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace snake_game.MainGame
{
    public class Fog
    {
        readonly Texture2D _gradientUp;
        readonly Texture2D _gradientLeft;
        public Fog(GraphicsDevice gd, Color a, Color b)
        {
            _gradientUp = new Texture2D(gd, 2, 2);
            _gradientUp.SetData(new []
            {
                a, a,
                b, b
            });
            _gradientLeft = new Texture2D(gd, 2, 2);
            _gradientLeft.SetData(new []
            {
                a, b,
                a, b
            });
        }

        public void CreateFog(SpriteBatch sb, Rectangle size, int distance)
        {
            sb.Draw(  // Top
                _gradientUp, new Rectangle(size.Left, size.Top, size.Width, distance),
                null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0
            );
            sb.Draw(  // Bottom
                _gradientUp, new Rectangle(size.Left, size.Bottom-distance, size.Width, distance),
                null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0
            );
            sb.Draw(  // Left
                _gradientLeft, new Rectangle(size.Left, size.Top, distance, size.Height),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0
            );
            sb.Draw(  // Right
                _gradientLeft, new Rectangle(size.Right-distance, size.Top, distance, size.Height),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0
            );
        }
    }
}