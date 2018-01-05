using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace Snake
{
    /// <inheritdoc />
    public class Config : IPluginConfig
    {
        /// <summary>
        ///     Расстояние между кружочками
        /// </summary>
        public int CircleOffset = 5;

        /// <summary>
        ///     Радиус кружочков
        /// </summary>
        public int CircleSize = 40;

        /// <summary>
        ///     Цвета змейки, если null, то разноцветная
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color[] Colors = null;

        /// <summary>
        ///     Тип управления змеёй
        /// </summary>
        public AvailableControllers ControlType = AvailableControllers.Small;

        /// <summary>
        ///     Цвет змеи, когда ей был нанесен урон и когда она неуязвима
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color DamageColor = Color.WhiteSmoke;

        /// <summary>
        ///     Через какое время неуязвимость пропадает, в мс.
        /// </summary>
        public int DamageTimeout = 1000;

        /// <summary>
        ///     Цвет головы змеи, если должен быть особенный. Может быть null, тогда используется <see cref="Colors" />
        /// </summary>
        [JsonConverter(typeof(HexColorConverter))]
        public Color? HeadColor = null;

        /// <summary>
        ///     Начальная длина змеи
        /// </summary>
        public int InitLen = 30;

        /// <summary>
        ///     Скорость змеи в пикселях в секундах
        /// </summary>
        public int Speed = 275;

        /// <summary>
        ///     На сколько змея должна поворачиваться.
        ///     Может быть null, если не нужно для выбранного типа управления. На данный момент используется только в
        ///     <see cref="ControllerSmall" />
        /// </summary>
        public int? TurnSize = 30;

        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;
    }
}