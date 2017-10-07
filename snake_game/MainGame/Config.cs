using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NullGuard;
using snake_game.Bonuses;

namespace snake_game.MainGame
{
    public class Config
    {
        public class SnakeConfigClass
        {
            public int Speed = 150; // Pixels per second
            public int CircleSize = 40; // Radius
            public int CircleOffset = 5;
            public int InitLen = 30;
            [JsonConverter(typeof(HexColorConverter))] public Color DamageColor = Color.WhiteSmoke;
            [JsonConverter(typeof(HexColorConverter))] public Color? HeadColor = null;
            [JsonConverter(typeof(HexColorConverter))] public Color[] Colors = null;
        }

        public class ScreenConfigClass
        {
            public bool IsMouseVisible = true;
            public bool IsFullScreen = false;
            public int ScreenWidth = 800;
            public int ScreenHeight = 600;
        }

        public class GameConfigClass
        {
            public int Lives = 3;
            public int DamageTimeout = 1500;
            public int FoodToLive = 10;
            [JsonConverter(typeof(HexColorConverter))] public Color TextColor = Color.Black;
            public bool DebugShow = false;
            [JsonConverter(typeof(HexColorConverter))] public Color DebugColor = Color.LightGray;
            public bool FogEnabled = true;

            [JsonConverter(typeof(HexColorConverter))]
            public Tuple<Color, Color> FogColor = new Tuple<Color, Color>(Color.DarkSlateBlue, Color.Transparent);

            public double FogSizeMultiplier = 1.5;
            [JsonConverter(typeof(HexColorConverter))] public Color BackgroundColor = Color.CornflowerBlue;
            public string ControlType = "small";
            public int? TurnSize = 30;
        }

        public SnakeConfigClass SnakeConfig = new SnakeConfigClass();
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

        [return: AllowNull]
        public override object ReadJson(JsonReader reader, Type objectType, [AllowNull] object existingValue,
            JsonSerializer serializer)
        {
            if (objectType.IsAssignableFrom(typeof(Color)))
            {
                var val = serializer.Deserialize<string>(reader);
                if (val == null)
                {
                    return null;
                }
                return FromString(val);
            }
            if (objectType.IsAssignableFrom(typeof(IEnumerable<Color>)))
            {
                var list = new List<Color>();
                foreach (var color in serializer.Deserialize<IEnumerable<string>>(reader).Select(FromString))
                    list.Add(color);
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
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typeSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var values = (Dictionary<string, IPluginConfig>) value;
            var result = new Dictionary<string, JObject>();
            foreach (var pair in values)
            {
                result[pair.Key] = JObject.FromObject(pair.Value, typeSerializer);
            }
            foreach (var pair in ConfigLoad.NotLoadedConfigs)
            {
                result[pair.Key] = pair.Value;
            }
            serializer.Serialize(writer, result);
        }

        public override object ReadJson(JsonReader reader, Type objectType, [AllowNull] object existingValue,
            JsonSerializer serializer)
        {
            var result = new Dictionary<string, IPluginConfig>();
            var dict = serializer.Deserialize<Dictionary<string, JObject>>(reader);
            foreach (var pair in dict)
            {
                var type = Type.GetType($"snake_plugins.{pair.Key}.Plugin");
                if (type != null && typeof(IPluginConfig).IsAssignableFrom(type))
                {
                    result[pair.Key] = (IPluginConfig)pair.Value.ToObject(type, serializer);
                }
                else
                {
                    // To save config without plugin
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
        
        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public static Config Parse(string json)
        {
            return JsonConvert.DeserializeObject<Config>(json, GetSettings());
        }

        public static string Save(Config config)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented, GetSettings());
        }
    }
}