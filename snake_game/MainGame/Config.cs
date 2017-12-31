using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using snake_game.Bonuses;

namespace snake_game.MainGame
{
    public class Config
    {
        public class SnakeConfigClass
        {
            public int Speed = 275; // Pixels per second
            public int CircleSize = 40; // Radius
            public int CircleOffset = 5;
            public int InitLen = 30;

            [JsonConverter(typeof(HexColorConverter))]
            public Color DamageColor = Color.WhiteSmoke;

            [JsonConverter(typeof(HexColorConverter))]
            public Color? HeadColor = null;

            [JsonConverter(typeof(HexColorConverter))]
            public Color[] Colors = null;
        }

        public class ScreenConfigClass
        {
            public bool IsMouseVisible = true;
            public bool IsFullScreen = false;

            // Most popular screen resolution minus Win10 taskbar and window borders
            public int ScreenWidth = 1366 - 2;

            public int ScreenHeight = 768 - 70;
        }

        public class GameConfigClass
        {
            public int Lives = 3;
            public int DamageTimeout = 1000;
            public int ScoreToLive = 7;

            [JsonConverter(typeof(HexColorConverter))]
            public Color TextColor = Color.Black;

            public bool FogEnabled = true;

            [JsonConverter(typeof(HexColorConverter))]
            public Tuple<Color, Color> FogColor = new Tuple<Color, Color>(Color.DarkSlateBlue, Color.Transparent);

            public double FogSize = 60;

            [JsonConverter(typeof(HexColorConverter))]
            public Color BackgroundColor = Color.CornflowerBlue;

            public int TurnSize { get; set; }
        }

        public ScreenConfigClass ScreenConfig = new ScreenConfigClass();
        public GameConfigClass GameConfig = new GameConfigClass();

        [JsonConverter(typeof(PluginConfigConverter))]
        public Dictionary<string, IPluginConfig> BonusConfig = new Dictionary<string, IPluginConfig>();
    }

    public class HexColorConverter : JsonConverter
    {
        public static string ToString(Color color)
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

        public static Color FromString(string color)
        {
            color = color.TrimStart('#');
            int r, g, b, a;
            switch (color.Length)
            {
                case 3: // rgb
                    // Multilple by 17, beacuse 0xF*17 = 15*17 = 255
                    r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    a = 255;
                    break;
                case 4: // rgba
                    r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    a = int.Parse(color.Substring(3, 1), System.Globalization.NumberStyles.HexNumber) * 17;
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Color color)
            {
                serializer.Serialize(writer, ToString(color));
            }
            else if (value is IEnumerable<Color> iEnumerable)
            {
                serializer.Serialize(writer, iEnumerable.Select(ToString).ToArray());
            }
            else if (value is Tuple<Color, Color> tuple)
            {
                serializer.Serialize(writer, new[]
                {
                    ToString(tuple.Item1), ToString(tuple.Item2)
                });
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                return null;
            }

            if (objectType.IsAssignableFrom(typeof(Color)))
            {
                var val = serializer.Deserialize<string>(reader);
                if (val == null)
                {
                    return null;
                }

                return FromString(val);
            }

            if (objectType.IsAssignableFrom(typeof(IEnumerable<Color>)) || objectType.IsAssignableFrom(typeof(Color[])))
            {
                var list = new List<Color>();
                foreach (var colorString in serializer.Deserialize<IEnumerable<string>>(reader))
                {
                    if (colorString != null)
                    {
                        list.Add(FromString(colorString));
                    }
                }

                return list.ToArray();
            }

            if (objectType.IsAssignableFrom(typeof(Tuple<Color, Color>)))
            {
                var items = serializer.Deserialize<string[]>(reader);
                return new Tuple<Color, Color>(
                    FromString(items[0]),
                    FromString(items[1])
                );
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Color)) ||
                   objectType.IsAssignableFrom(typeof(IEnumerable<Color>)) ||
                   objectType.IsAssignableFrom(typeof(Tuple<Color, Color>));
        }
    }

    public class PluginConfigConverter : JsonConverter
    {
        private const string IronPythonType = "<IronPython>";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var values = (Dictionary<string, IPluginConfig>) value;
            var result = new Dictionary<string, JObject>();
            foreach (var pair in ConfigLoad.NotLoadedConfigs)
            {
                result[pair.Key] = pair.Value;
            }

            foreach (var pair in values)
            {
                if (ConfigLoad.PluginParsers.TryGetValue(pair.Key, out var parser))
                {
                    var newValue = parser.SaveConfig(pair.Value, serializer);
                    result[pair.Key] = newValue;
                }
            }

            serializer.Serialize(writer, result);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var result = new Dictionary<string, IPluginConfig>();
            var dict = serializer.Deserialize<Dictionary<string, JObject>>(reader);
            foreach (var pair in dict)
            {
                var pluginConfig = ConfigLoad.PluginParsers.TryGetValue(pair.Key, out var parser)
                    ? parser.LoadConfig(pair.Value, serializer)
                    : null;
                if (pluginConfig != null)
                {
                    result[pair.Key] = pluginConfig;
                }
                else
                {
                    ConfigLoad.NotLoadedConfigs[pair.Key] = pair.Value;
                }
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(IDictionary<string, IPluginConfig>));
        }
    }

    public static class ConfigLoad
    {
        public static Dictionary<string, JObject> NotLoadedConfigs = new Dictionary<string, JObject>();
        public static IDictionary<string, IPluginContainer> PluginParsers { get; private set; }

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public static Config Parse(string json, IDictionary<string, IPluginContainer> pluginParsers)
        {
            PluginParsers = pluginParsers;
            var result = JsonConvert.DeserializeObject<Config>(json, GetSettings());
            PluginParsers = null;
            return result;
        }

        public static string Save(Config config, IDictionary<string, IPluginContainer> pluginParsers)
        {
            PluginParsers = pluginParsers;
            var result = JsonConvert.SerializeObject(config, Formatting.Indented, GetSettings());
            PluginParsers = null;
            return result;
        }
    }
}