using snake_game.Bonuses;
using snake_game.Utils;

namespace Snake
{
    /// <inheritdoc />
    /// <summary>
    ///     События змейки
    /// </summary>
    public class SnakeEvents : Accessable
    {
        /// <summary>
        ///     Информация о нанесенном уроне
        /// </summary>
        public Damaged DamageInfo { [Accessable] get; } = new Damaged();

        /// <summary>
        ///     Неуязвима ли змейка. Если null, то расчитывается относительно времени последнего нанесения урона согласно конфигу.
        ///     <seealso cref="Bonus.Damage(int)" />
        /// </summary>
        public bool? Invulnerable { [Accessable] get; [Accessable] set; }

        /// <inheritdoc />
        public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
        {
            switch (methodName)
            {
                default:
                    return base.GetMethodResult<TResult>(methodName, arguments);
            }
        }

        /// <inheritdoc />
        public override void SetProperty(string propertyName, object newValue)
        {
            switch (propertyName)
            {
                case nameof(Invulnerable):
                    Invulnerable = (bool) newValue;
                    break;
                default:
                    base.SetProperty(propertyName, newValue);
                    break;
            }
        }

        /// <inheritdoc />
        public override TResult GetProperty<TResult>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Invulnerable):
                    return (TResult) (object) Invulnerable;
                case nameof(DamageInfo):
                    return (TResult) (object) DamageInfo;
                default:
                    return base.GetProperty<TResult>(propertyName);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Содержит информацию о уроне.
        /// </summary>
        public class Damaged : Accessable
        {
            /// <summary>
            ///     В первый ли раз змейка получает это событие
            /// </summary>
            public bool FirstTime = true;

            /// <summary>
            ///     Сколько урона было нанесено
            /// </summary>
            public int Damage { [Accessable] get; set; }

            /// <summary>
            ///     Был ли этот урон отменен
            /// </summary>
            public bool Prevented { [Accessable] get; set; }

            /// <summary>
            ///     Отменяет нанесение урона
            /// </summary>
            /// <returns></returns>
            [Accessable]
            public Void Prevent()
            {
                Prevented = true;
                return new Void();
            }

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

            /// <inheritdoc />
            public override TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
            {
                switch (methodName)
                {
                    case nameof(Prevent):
                        return (TResult) (object) Prevent();
                    default:
                        return base.GetMethodResult<TResult>(methodName, arguments);
                }
            }
        }
    }
}