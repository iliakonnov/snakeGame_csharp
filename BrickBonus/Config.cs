using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace BrickBonus
{
    /// <inheritdoc />
    public class Config : IPluginConfig
    {
        /// <summary>
        ///     Цвет кирпичей
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color BrickColor = Color.OrangeRed;

        /// <summary>
        ///     Промежуток времени между случайными событиями, напрмер появлением нового кирпича или их движением
        /// </summary>
        public int ChanceTime = 1500;

        /// <summary>
        ///     Шанс, что кирпич подвинется. От 0 до 1
        /// </summary>
        public double MoveChance = 0.75;

        /// <summary>
        ///     Шанс, что появится новый кирпич. От 0 до 1
        /// </summary>
        public double NewChance = 0.25;

        /// <summary>
        ///     Размер кирпичей
        /// </summary>
        public int Size = 25;

        /// <summary>
        ///     На сколько пикселей кирпичи будут двигаться
        /// </summary>
        public int Step = 50;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;
    }
}