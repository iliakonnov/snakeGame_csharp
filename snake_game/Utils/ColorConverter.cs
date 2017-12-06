using Microsoft.Xna.Framework;

namespace snake_game.Utils
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

        public static Eto.Drawing.Color ToEto(System.Drawing.Color color)
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
                (int) (color.R * 255.0f),
                (int) (color.G * 255.0f),
                (int) (color.B * 255.0f),
                (int) (color.A * 255.0f)
            );
        }

        public static Color ToXna(System.Drawing.Color color)
        {
            return new Color(
                color.R,
                color.G,
                color.B,
                color.A
            );
        }

        public static System.Drawing.Color ToSystem(Color color)
        {
            return System.Drawing.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B
            );
        }

        public static System.Drawing.Color ToSystem(Eto.Drawing.Color color)
        {
            return System.Drawing.Color.FromArgb(
                (int) (color.A * 255.0f),
                (int) (color.R * 255.0f),
                (int) (color.G * 255.0f),
                (int) (color.B * 255.0f)
            );
        }
    }
}