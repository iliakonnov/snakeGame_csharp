using Microsoft.Xna.Framework;

namespace snake_game.Launcher
{
    public static class ColorConverter
    {
        public static Eto.Drawing.Color ToEto(Color color)
        {
            return new Eto.Drawing.Color(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                color.A / 255.0f
            );
        }
        public static Color ToXna(Eto.Drawing.Color color)
        {
            return new Color(
                color.R * 255,
                color.G * 255,
                color.B * 255,
                color.A * 255
            );
        }
    }
}