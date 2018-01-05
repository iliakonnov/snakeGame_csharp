using Microsoft.Xna.Framework.Input;

namespace Snake
{
    /// <summary>
    ///     Все доступные типы управления
    /// </summary>
    public enum AvailableControllers
    {
        /// <summary>
        ///     Соответствует <see cref="ControllerTraditional" />
        /// </summary>
        Traditional,

        /// <summary>
        ///     Соответствует <see cref="ControllerSmall" />
        /// </summary>
        Small
    }

    /// <summary>
    ///     Результат работы <see cref="IController" />
    /// </summary>
    public class TurnResult
    {
        /// <summary>
        ///     Нужно ли <see cref="TurnDegrees" /> не прибавить к текущему значению поворота, а заменить
        /// </summary>
        public bool ReplaceTurn;

        /// <summary>
        ///     Нужно ли поворачивать
        /// </summary>
        public bool ToTurn;

        /// <summary>
        ///     На сколько именно надо повернуть
        /// </summary>
        public int TurnDegrees;
    }

    /// <summary>
    ///     Должен реализовывать любой тип управления
    /// </summary>
    public interface IController
    {
        /// <summary>
        ///     Получает результат управления, на основе текущего состояния клавиатуры
        /// </summary>
        /// <param name="state">Состояние клавиатуры</param>
        /// <returns></returns>
        TurnResult Control(KeyboardState state);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Управление как в обычной змейке. Влево, вправо, вверх и вниз
    /// </summary>
    public class ControllerTraditional : IController
    {
        private Direction _direction = Direction.Right;

        /// <inheritdoc />
        public TurnResult Control(KeyboardState state)
        {
            var result = new TurnResult();

            if (state.IsKeyDown(Keys.Down) && _direction != Direction.Up)
            {
                result = new TurnResult
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 90
                };
                _direction = Direction.Down;
            }
            else if (state.IsKeyDown(Keys.Left) && _direction != Direction.Right)
            {
                result = new TurnResult
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 180
                };
                _direction = Direction.Left;
            }
            else if (state.IsKeyDown(Keys.Up) && _direction != Direction.Down)
            {
                result = new TurnResult
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 270
                };
                _direction = Direction.Up;
            }
            else if (state.IsKeyDown(Keys.Right) && _direction != Direction.Left)
            {
                result = new TurnResult
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 0
                };
                _direction = Direction.Right;
            }

            return result;
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Управление маленькими изменениями направления влево или вправо.
    /// </summary>
    public class ControllerSmall : IController
    {
        private readonly int _step;
        private bool _isTurned;

        /// <inheritdoc />
        public ControllerSmall(int step)
        {
            _step = step;
        }

        /// <inheritdoc />
        public TurnResult Control(KeyboardState state)
        {
            var result = new TurnResult();

            if (state.IsKeyDown(Keys.Right))
            {
                if (!_isTurned)
                {
                    _isTurned = true;
                    result = new TurnResult
                    {
                        ToTurn = true,
                        ReplaceTurn = false,
                        TurnDegrees = _step
                    };
                }
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                if (!_isTurned)
                {
                    _isTurned = true;
                    result = new TurnResult
                    {
                        ToTurn = true,
                        ReplaceTurn = false,
                        TurnDegrees = -_step
                    };
                }
            }
            else
            {
                _isTurned = false;
            }

            return result;
        }
    }
}