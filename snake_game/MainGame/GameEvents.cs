using snake_game.Bonuses;

namespace snake_game.MainGame
{
    /// <inheritdoc />
    /// <summary>
    ///     События ядра игры
    /// </summary>
    public class GameEvents : Accessable
    {
        /// <summary>
        ///     Количество урона, нанесенного за тик
        /// </summary>
        public int Damage;

        /// <inheritdoc />
        public override TResult GetProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Damage):
                    return (TResult) (object) Damage;
                default:
                    return base.GetProperty<TResult>(propertyName);
            }
        }
    }
}