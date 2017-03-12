using Microsoft.Xna.Framework.Input;

namespace snake_game.MainGame
{
	public static class ControlResult
	{
		public class Result
		{
			public bool Debug;
			public bool IsExit;
			public Turn Turn = new Turn();
		}
		public class Turn
		{
			public bool ToTurn;
			public bool ReplaceTurn;
			public int TurnDegrees;
		}
	}

	public class Controller
	{
		bool _IsTurned;
		bool _IsDebugPressed;
		readonly int _step;

		public Controller(int step)
		{
			_step = step;
		}

		public ControlResult.Result Control(KeyboardState state)
		{
			var result = new ControlResult.Result();

			if (state.IsKeyDown(Keys.OemTilde))
			{
				if (!_IsDebugPressed)
				{
					result.Debug = true;
					_IsDebugPressed = true;
				}
			}
			else
			{
				_IsDebugPressed = false;
			}

			if (state.IsKeyDown(Keys.Escape))
			{
				result.IsExit = true;
			}

			/*if (state.IsKeyDown(Keys.Down))
			{
				result.Turn = new ControlResult.Turn
				{
					ToTurn = true,
					ReplaceTurn = true,
					TurnDegrees = 270
				};
			}
			else if (state.IsKeyDown(Keys.Left))
			{
				result.Turn = new ControlResult.Turn
				{
					ToTurn = true,
					ReplaceTurn = true,
					TurnDegrees = 180
				};
			}
			else if (state.IsKeyDown(Keys.Up))
			{
				result.Turn = new ControlResult.Turn
				{
					ToTurn = true,
					ReplaceTurn = true,
					TurnDegrees = 90
				};
			}
			else if (state.IsKeyDown(Keys.Right))
			{
				result.Turn = new ControlResult.Turn
				{
					ToTurn = true,
					ReplaceTurn = true,
					TurnDegrees = 0
				};
			}*/

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