namespace snake_game
{
    static class Program
	{
		static void Main(string[] args)
		{
		    var l = args.Length;
		    if (l >= 0 || l < 0)
		    {
		        var game = new MainGame.MainGame();
		        game.Run();
		    }
		}
	}
}
