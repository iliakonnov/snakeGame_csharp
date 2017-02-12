using Microsoft.Xna.Framework.Input;

namespace snake_game.MainGame
{
    public static class ControlResult
    {
        public class Result
        {
            public bool IsExit = false;
            public Turn Turn = new Turn();
        }
        public class Turn
        {
            public bool ToTurn = false;
            public bool ReplaceTurn;
            public int TurnDegrees;
        }
    }

    public static class Controller
    {
        public static ControlResult.Result Control(KeyboardState state)
        {
            var result = new ControlResult.Result();

            if (state.IsKeyDown(Keys.Escape))
            {
                result.IsExit = true;
            }

            if (state.IsKeyDown(Keys.Down))
            {
                result.Turn = new ControlResult.Turn
                {
                    ReplaceTurn = true,
                    TurnDegrees = 270
                };
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                result.Turn = new ControlResult.Turn
                {
                    ReplaceTurn = true,
                    TurnDegrees = 180
                };
            }
            else if (state.IsKeyDown(Keys.Up))
            {
                result.Turn = new ControlResult.Turn
                {
                    ReplaceTurn = true,
                    TurnDegrees = 90
                };
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                result.Turn = new ControlResult.Turn
                {
                    ReplaceTurn = true,
                    TurnDegrees = 0
                };
            }

            return result;
        }
    }
}