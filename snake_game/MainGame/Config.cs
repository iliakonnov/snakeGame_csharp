using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace snake_game.MainGame
{
    public class Config
    {
        public class SnakeConfigClass
        {
            public int CircleSize = 40;
            public int CircleOffset = 5;
            public int InitLen = 30;
            public Color[] Colors = null;
        }

        public class ScreenConfigClass
        {
            public bool IsMouseVisible = true;
            public bool IsFullScreen;
            public int ScreenWidth = 800;
            public int ScreenHeight = 600;
        }

        public class GameConfigClass
        {
            public bool DebugShow;
            public Color DebugColor = Color.LightGray;
            public bool FogEnabled = true;
            public Tuple<Color,Color> FogColor = new Tuple<Color, Color>(Color.DarkSlateBlue, Color.Transparent);
        }

        public class BonusConfigClass
        {
            public bool BonusEnable = true;
        }
        public SnakeConfigClass SnakeConfig = new SnakeConfigClass();
        public ScreenConfigClass ScreenConfig = new ScreenConfigClass();
        public GameConfigClass GameConfig = new GameConfigClass();
        public BonusConfigClass BonusConfig = new BonusConfigClass();
    }

    public static class HexColorConverter
    {
        // TODO: Разобраться с json и конвертировать цвета с помощью этого класса.
        public static string ToString(Color color)
        {
            var r = color.R.ToString("X");
            var g = color.G.ToString("X");
            var b = color.B.ToString("X");
            var a = color.A.ToString("X");

            if (a == "FF") a = "";

            if (
                r[0] == r[1] &&
                g[0] == g[1] &&
                b[0] == b[1] &&
                (a[0] == a[1] || a == "")
            )
            {
                r = r[0].ToString();
                g = g[0].ToString();
                b = b[0].ToString();
                if (a != "") a = a[0].ToString();
            }

            return $"#{r}{g}{b}{a}";
        }

        public static Color FromString(string color)
        {
            color = color.TrimStart('#');
            int r, g, b, a;
            switch (color.Length)
            {
                case 3: // rgb
                    r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
                    a = 255;
                    break;
                case 4: // rgba
                    r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
                    a = int.Parse(color.Substring(3, 1), System.Globalization.NumberStyles.HexNumber);
                    break;
                case 6: // rrggbb
                    r = int.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    a = 255;
                    break;
                case 8: // rrggbbaa
                    r = int.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = int.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = int.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    a = int.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    break;
                default:
                    throw new ArgumentException("Unknown color format");
            }
            return new Color(r, g, b, a);
        }
    }

    public class ConfigLoad
    {
        public static Config Parse(string json)
        {
            return JsonConvert.DeserializeObject<Config>(json);
        }

        public static string Save(Config config)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }
    }
}