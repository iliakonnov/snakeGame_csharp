using System;
namespace snake_game
{
	static class Program
	{
		private static Game1 game;

		internal static void RunGame()
		{
			game = new Game1();
			game.Run();
		}

		static void Main(string[] args)
		{
			RunGame();
		}
	}
}
