using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game.MainGame
{
    /// <summary>
    ///     Туман по краям карты
    /// </summary>
    public class Fog
    {
        private readonly Texture2D _gradientLeft;
        private readonly Texture2D _gradientUp;

        /// <inheritdoc />
        /// <param name="gd">Графическое устройство</param>
        /// <param name="a">Цвет начала градиента (ближе к краям)</param>
        /// <param name="b">Цвет конца градиента (ближе к центру)</param>
        public Fog(GraphicsDevice gd, Color a, Color b)
        {
            _gradientUp = new Texture2D(gd, 2, 2);
            _gradientUp.SetData(new[]
            {
                a, a,
                b, b
            });
            _gradientLeft = new Texture2D(gd, 2, 2);
            _gradientLeft.SetData(new[]
            {
                a, b,
                a, b
            });
        }

        /// <summary>
        ///     Отрисовывает туман
        /// </summary>
        /// <param name="sb">Графическое устройство</param>
        /// <param name="size">Размер экрана</param>
        /// <param name="distance">Размер тумана (в px)</param>
        public void CreateFog(SpriteBatch sb, Rectangle size, int distance)
        {
            sb.Draw( // Top
                _gradientUp, new Rectangle(size.Left, size.Top, size.Width, distance),
                null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0
            );
            sb.Draw( // Bottom
                _gradientUp, new Rectangle(size.Left, size.Bottom - distance, size.Width, distance),
                null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0
            );
            sb.Draw( // Left
                _gradientLeft, new Rectangle(size.Left, size.Top, distance, size.Height),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0
            );
            sb.Draw( // Right
                _gradientLeft, new Rectangle(size.Right - distance, size.Top, distance, size.Height),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0
            );
        }
    }
}