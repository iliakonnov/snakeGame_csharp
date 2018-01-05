using System;
using System.Globalization;
using Eto.Drawing;

namespace snake_game.Utils
{
    /// <summary>
    ///     Преобразовывает цвета в различные форматы
    /// </summary>
    public static class ColorConverter
    {
        /// <summary>
        ///     Преобразовывает цвет XNA в цвет Eto
        /// </summary>
        /// <param name="color">Цвет XNA</param>
        /// <returns>Цвет Eto</returns>
        public static Color ToEto(Microsoft.Xna.Framework.Color color)
        {
            return new Color(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                color.A / 255.0f
            );
        }

        /// <summary>
        ///     Преобразовывает цвет System в цвет Eto
        /// </summary>
        /// <param name="color">Цвет System</param>
        /// <returns>Цвет Eto</returns>
        public static Color ToEto(System.Drawing.Color color)
        {
            return new Color(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                color.A / 255.0f
            );
        }

        /// <summary>
        ///     Преобразовывает цвет Eto в цвет XNA
        /// </summary>
        /// <param name="color">Цвет Eto</param>
        /// <returns>Цвет XNA</returns>
        public static Microsoft.Xna.Framework.Color ToXna(Color color)
        {
            return new Microsoft.Xna.Framework.Color(
                (int) (color.R * 255.0f),
                (int) (color.G * 255.0f),
                (int) (color.B * 255.0f),
                (int) (color.A * 255.0f)
            );
        }

        /// <summary>
        ///     Преобразовывает цвет System в цвет XNA
        /// </summary>
        /// <param name="color">Цвет System</param>
        /// <returns>Цвет XNA</returns>
        public static Microsoft.Xna.Framework.Color ToXna(System.Drawing.Color color)
        {
            return new Microsoft.Xna.Framework.Color(
                color.R,
                color.G,
                color.B,
                color.A
            );
        }

        /// <summary>
        ///     Преобразовывает цвет XNA в цвет System
        /// </summary>
        /// <param name="color">Цвет XNA</param>
        /// <returns>Цвет System</returns>
        public static System.Drawing.Color ToSystem(Microsoft.Xna.Framework.Color color)
        {
            return System.Drawing.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B
            );
        }

        /// <summary>
        ///     Преобразовывает цвет Eto в цвет System
        /// </summary>
        /// <param name="color">Цвет Eto</param>
        /// <returns>Цвет System</returns>
        public static System.Drawing.Color ToSystem(Color color)
        {
            return System.Drawing.Color.FromArgb(
                (int) (color.A * 255.0f),
                (int) (color.R * 255.0f),
                (int) (color.G * 255.0f),
                (int) (color.B * 255.0f)
            );
        }

        /// <summary>
        ///     Преобразовывает цвет Eto в цвет CSS
        /// </summary>
        /// <param name="color">Цвет Eto</param>
        /// <returns>Цвет CSS</returns>
        public static string ToCSS(Color color)
        {
            return ToCSS(ToXna(color));
        }

        /// <summary>
        ///     Преобразовывает цвет System в цвет CSS
        /// </summary>
        /// <param name="color">Цвет System</param>
        /// <returns>Цвет CSS</returns>
        public static string ToCSS(System.Drawing.Color color)
        {
            return ToCSS(ToXna(color));
        }

        /// <summary>
        ///     Преобразовывает цвет XNA в цвет CSS
        /// </summary>
        /// <param name="color">Цвет XNA</param>
        /// <returns>Цвет CSS</returns>
        public static string ToCSS(Microsoft.Xna.Framework.Color color)
        {
            var r = color.R.ToString("X").PadLeft(2, '0');
            var g = color.G.ToString("X").PadLeft(2, '0');
            var b = color.B.ToString("X").PadLeft(2, '0');
            var a = color.A.ToString("X").PadLeft(2, '0');

            if (a == "FF") a = "";

            if (
                (r.Length == 1 || r[0] == r[1]) &&
                (g.Length == 1 || g[0] == g[1]) &&
                (b.Length == 1 || b[0] == b[1]) &&
                (a == "" || a.Length == 1 || a[0] == a[1])
            )
            {
                r = r[0].ToString();
                g = g[0].ToString();
                b = b[0].ToString();
                if (a != "") a = a[0].ToString();
            }

            return $"#{r}{g}{b}{a}";
        }

        /// <summary>
        ///     Преобразовывает цвет CSS в цвет XNA
        /// </summary>
        /// <param name="color">Цвет CSS</param>
        /// <returns>Цвет XNA</returns>
        /// <exception cref="ArgumentException">В случае, если была передана неверная строка</exception>
        public static Microsoft.Xna.Framework.Color FromCSS(string color)
        {
            color = color.TrimStart('#');
            int r, g, b, a;
            switch (color.Length)
            {
                case 3: // rgb
                    // Multilple by 17, beacuse 0xF*17 = 15*17 = 255
                    r = int.Parse(color.Substring(0, 1), NumberStyles.HexNumber) * 17;
                    g = int.Parse(color.Substring(1, 1), NumberStyles.HexNumber) * 17;
                    b = int.Parse(color.Substring(2, 1), NumberStyles.HexNumber) * 17;
                    a = 255;
                    break;
                case 4: // rgba
                    r = int.Parse(color.Substring(0, 1), NumberStyles.HexNumber) * 17;
                    g = int.Parse(color.Substring(1, 1), NumberStyles.HexNumber) * 17;
                    b = int.Parse(color.Substring(2, 1), NumberStyles.HexNumber) * 17;
                    a = int.Parse(color.Substring(3, 1), NumberStyles.HexNumber) * 17;
                    break;
                case 6: // rrggbb
                    r = int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                    a = 255;
                    break;
                case 8: // rrggbbaa
                    r = int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                    a = int.Parse(color.Substring(6, 2), NumberStyles.HexNumber);
                    break;
                default:
                    throw new ArgumentException("Unknown color format");
            }

            return new Microsoft.Xna.Framework.Color(r, g, b, a);
        }
    }
}