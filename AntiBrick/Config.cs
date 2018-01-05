using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AntiBrick
{
    /// <inheritdoc />
    public class Config : IPluginConfig
    {
        /// <summary>
        ///     Каждые сколько миллисекунд будет попытка появления бонуса
        /// </summary>
        public int ChanceTime = 7000;

        /// <summary>
        ///     Цвет бонуса
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color Color = Color.GreenYellow;

        /// <summary>
        ///     Вероятность появления бонуса. От 1 до 0
        /// </summary>
        public double NewChance = 0.2;

        /// <summary>
        ///     Размер бонуса
        /// </summary>
        public int Size = 25;

        /// <summary>
        ///     Количесвто кирпичей, начиная с которого будет вроятность появления бонуса
        /// </summary>
        public int StartBrickCount = 10;

        /// <summary>
        ///     Толщина границ бонуса
        /// </summary>
        public float Thickness = 10f;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;
    }
}