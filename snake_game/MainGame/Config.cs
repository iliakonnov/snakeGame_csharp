using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using snake_game.Bonuses;
using snake_game.Utils;

namespace snake_game.MainGame
{
    /// <summary>
    ///     Настройки игры
    /// </summary>
    public class Config
    {
        /// <summary>
        ///     Настройки бонусов. Ключ - имя плагина.
        /// </summary>
        [JsonConverter(typeof(PluginConfigConverter))]
        public Dictionary<string, IPluginConfig> BonusConfig = new Dictionary<string, IPluginConfig>();

        /// <summary>
        ///     Содержимое настроек ядра игры
        /// </summary>
        public GameConfigClass GameConfig = new GameConfigClass();

        /// <summary>
        ///     Содержимое настроек экрана
        /// </summary>
        public ScreenConfigClass ScreenConfig = new ScreenConfigClass();

        /// <summary>
        ///     Настройки отображения
        /// </summary>
        public class ScreenConfigClass
        {
            /// <summary>
            ///     Включить ли полноэкранный режим
            ///     <seealso cref="GraphicsDeviceManager.IsFullScreen" />
            /// </summary>
            public bool IsFullScreen = false;

            /// <summary>
            ///     Показывать ли курсор
            ///     <seealso cref="Game.IsMouseVisible" />
            /// </summary>
            public bool IsMouseVisible = true;

            /// <summary>
            ///     Высота окна игры
            ///     <seealso cref="GraphicsDeviceManager.PreferredBackBufferHeight" />
            /// </summary>
            public int ScreenHeight = 768 - 70;

            // Most popular screen resolution minus Win10 taskbar and window borders
            /// <summary>
            ///     Ширина окна игры
            ///     <seealso cref="GraphicsDeviceManager.PreferredBackBufferWidth" />
            /// </summary>
            public int ScreenWidth = 1366 - 2;
        }

        /// <summary>
        ///     Настройки ядра игры
        /// </summary>
        public class GameConfigClass
        {
            /// <summary>
            ///     Цвет фона игры
            /// </summary>
            [JsonConverter(typeof(HexColorConverter))]
            public Color BackgroundColor = Color.CornflowerBlue;

            /// <summary>
            ///     Цвет градиента тумана. Первое - цвет ближе к краям, второе - цвет ближе к центру
            ///     <seealso cref="Fog(GraphicsDevice,Color,Color)" />
            /// </summary>
            [JsonConverter(typeof(HexColorConverter))]
            public Tuple<Color, Color> FogColor = new Tuple<Color, Color>(Color.DarkSlateBlue, Color.Transparent);

            /// <summary>
            ///     Включен ли туман
            ///     <seealso cref="Fog" />
            /// </summary>
            public bool FogEnabled = true;

            /// <summary>
            ///     Размер тумана (в px)
            ///     <seealso cref="Fog.CreateFog(SpriteBatch,Rectangle,int)" />
            /// </summary>
            public double FogSize = 60;

            /// <summary>
            ///     Начальное количество жизней
            /// </summary>
            public int Lives = 3;

            /// <summary>
            ///     Сколько ннажо набрать очков, чтоы получить жизнь
            /// </summary>
            public int ScoreToLive = 7;

            /// <summary>
            ///     Цвет текста информации (жизни и счет)
            /// </summary>
            [JsonConverter(typeof(HexColorConverter))]
            public Color TextColor = Color.Black;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Преобразовывает цвета в строку цвета CSS
    /// </summary>
    public class HexColorConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case Color color:
                    serializer.Serialize(writer, ColorConverter.ToCSS(color));
                    break;
                case IEnumerable<Color> iEnumerable:
                    serializer.Serialize(writer, iEnumerable.Select(ColorConverter.ToCSS).ToArray());
                    break;
                case Tuple<Color, Color> tuple:
                    serializer.Serialize(writer, new[]
                    {
                        ColorConverter.ToCSS(tuple.Item1), ColorConverter.ToCSS(tuple.Item2)
                    });
                    break;
            }
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (existingValue == null) return null;

            if (objectType.IsAssignableFrom(typeof(Color)))
            {
                var val = serializer.Deserialize<string>(reader);
                if (val == null) return null;

                return ColorConverter.FromCSS(val);
            }

            if (objectType.IsAssignableFrom(typeof(IEnumerable<Color>)) || objectType.IsAssignableFrom(typeof(Color[])))
                return (from colorString in serializer.Deserialize<IEnumerable<string>>(reader)
                    where colorString != null
                    select ColorConverter.FromCSS(colorString)).ToArray();

            if (objectType.IsAssignableFrom(typeof(Tuple<Color, Color>)))
            {
                var items = serializer.Deserialize<string[]>(reader);
                return new Tuple<Color, Color>(
                    ColorConverter.FromCSS(items[0]),
                    ColorConverter.FromCSS(items[1])
                );
            }

            return null;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(Color)) ||
                   objectType.IsAssignableFrom(typeof(IEnumerable<Color>)) ||
                   objectType.IsAssignableFrom(typeof(Tuple<Color, Color>));
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Загружает из JSON и сохраняет в него же настройки плагинов.
    ///     <seealso cref="IPluginContainer" />
    /// </summary>
    public class PluginConfigConverter : JsonConverter
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var values = (Dictionary<string, IPluginConfig>) value;
            var result = new Dictionary<string, JObject>();
            foreach (var pair in ConfigLoad.NotLoadedConfigs) result[pair.Key] = pair.Value;

            foreach (var pair in values)
                if (ConfigLoad.PluginParsers.TryGetValue(pair.Key, out var parser))
                {
                    var newValue = parser.SaveConfig(pair.Value, serializer);
                    result[pair.Key] = newValue;
                }

            serializer.Serialize(writer, result);
        }

        /// <inheritdoc />
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
                    result[pair.Key] = pluginConfig;
                else
                    ConfigLoad.NotLoadedConfigs[pair.Key] = pair.Value;
            }

            return result;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(IDictionary<string, IPluginConfig>));
        }
    }

    /// <summary>
    ///     Содержит всё необходимое для работы с настройками игры
    /// </summary>
    public static class ConfigLoad
    {
        /// <summary>
        ///     Настройки тех плагинов, которые не удалось загрузить. Используется в <see cref="PluginConfigConverter" />
        /// </summary>
        public static readonly Dictionary<string, JObject> NotLoadedConfigs = new Dictionary<string, JObject>();

        /// <summary>
        ///     Обработчики объектов JSON для каждого плагина. Используется в <see cref="PluginConfigConverter" />
        /// </summary>
        public static IDictionary<string, IPluginContainer> PluginParsers { get; private set; }

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        /// <summary>
        ///     Загружает все настройки игры
        /// </summary>
        /// <param name="json">Строка, содержащая json с параметрами</param>
        /// <param name="pluginParsers">Загруженые плагины</param>
        /// <returns>Настройки игры</returns>
        public static Config Parse(string json, IDictionary<string, IPluginContainer> pluginParsers)
        {
            PluginParsers = pluginParsers;
            var result = JsonConvert.DeserializeObject<Config>(json, GetSettings());
            PluginParsers = null;
            return result;
        }

        /// <summary>
        ///     Сохраняет настройки игры в json
        /// </summary>
        /// <param name="config">Настройки игры</param>
        /// <param name="pluginParsers">Загруженные плагины</param>
        /// <returns>Настройки в json</returns>
        public static string Save(Config config, IDictionary<string, IPluginContainer> pluginParsers)
        {
            PluginParsers = pluginParsers;
            var result = JsonConvert.SerializeObject(config, Formatting.Indented, GetSettings());
            PluginParsers = null;
            return result;
        }
    }
}