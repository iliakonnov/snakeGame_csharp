using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AntiAppleBonus
{
    /// <inheritdoc />
    public class Config : IPluginConfig
    {
        /// <summary>
        ///     Каждые сколько миллисекунд пытаться создать бонус
        /// </summary>
        public int ChanceTime = 7000;

        /// <summary>
        ///     Цвет бонуса
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color Color = Color.LightYellow;

        /// <summary>
        ///     Вероятность появления бонуса, от 0 до 1
        /// </summary>
        public double NewChance = 0.2;

        /// <summary>
        ///     Размер бонуса
        /// </summary>
        public int Size = 25;

        /// <summary>
        ///     Минимальная длина змеи, начиная с которой есть вероятность появления бонуса
        /// </summary>
        public int StartSnakeLength = 20;

        /// <summary>
        ///     Толщина границ бонуса
        /// </summary>
        public float Thickness = 10f;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;
    }
}