using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public static class ControlResult
    {
        public class Result
        {
            public Turn Turn = new Turn();
        }

        public class Turn
        {
            public bool ToTurn;
            public bool ReplaceTurn;
            public int TurnDegrees;
        }
    }

    public interface IController
    {
        ControlResult.Result Control(KeyboardState state);
    }

    public class ControllerTraditional : IController
    {
        enum Direction { Up, Down, Left, Right }
        Direction _direction = Direction.Right;

        public ControlResult.Result Control(KeyboardState state)
        {
            var result = new ControlResult.Result();

            if (state.IsKeyDown(Keys.Down) && _direction != Direction.Up)
            {
                result.Turn = new ControlResult.Turn
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 90
                };
                _direction = Direction.Down;
            }
            else if (state.IsKeyDown(Keys.Left) && _direction != Direction.Right)
            {
                result.Turn = new ControlResult.Turn
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 180
                };
                _direction = Direction.Left;
            }
            else if (state.IsKeyDown(Keys.Up) && _direction != Direction.Down)
            {
                result.Turn = new ControlResult.Turn
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 270
                };
                _direction = Direction.Up;
            }
            else if (state.IsKeyDown(Keys.Right) && _direction != Direction.Left)
            {
                result.Turn = new ControlResult.Turn
                {
                    ToTurn = true,
                    ReplaceTurn = true,
                    TurnDegrees = 0
                };
                _direction = Direction.Right;
            }

            return result;
        }
    }

    public class ControllerSmall : IController
    {
        bool _IsTurned;
        readonly int _step;

        public ControllerSmall(int step)
        {
            _step = step;
        }

        public ControlResult.Result Control(KeyboardState state)
        {
            var result = new ControlResult.Result();

            if (state.IsKeyDown(Keys.Right))
            {
                if (!_IsTurned)
                {
                    _IsTurned = true;
                    result.Turn = new ControlResult.Turn
                    {
                        ToTurn = true,
                        ReplaceTurn = false,
                        TurnDegrees = _step
                    };
                }
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                if (!_IsTurned)
                {
                    _IsTurned = true;
                    result.Turn = new ControlResult.Turn
                    {
                        ToTurn = true,
                        ReplaceTurn = false,
                        TurnDegrees = -_step
                    };
                }
            }
            else
            {
                _IsTurned = false;
            }

            return result;
        }
    }
}