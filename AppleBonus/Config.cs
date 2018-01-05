using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AppleBonus
{
    /// <inheritdoc />
    public class Config : IPluginConfig
    {
        /// <summary>
        ///     Цвет яблок
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color AppleColor = Color.SpringGreen;

        /// <summary>
        ///     Количество яблок в игре
        /// </summary>
        public int AppleCount = 2;

        /// <summary>
        ///     Задержка проверки отскока от препятствий в мс
        /// </summary>
        public float BounceTimeout = 150f;

        /// <summary>
        ///     Радиус яблока
        /// </summary>
        public int Radius = 25;

        /// <summary>
        ///     Качество окружности
        /// </summary>
        public int Sides = 30;

        /// <summary>
        ///     Скорость движения яблок
        /// </summary>
        public int Speed = 300;

        /// <summary>
        ///     Толщина границ
        /// </summary>
        public float Thickness = 10f;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;
    }
}