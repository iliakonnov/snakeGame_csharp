using System;
using System.IO;
using snake_game.Launcher;
using snake_game.MainGame;

namespace snake_game
{
	static class Program
	{
		/*static void Main(string[] args)
		{
			string cfg;
			switch (args.Length)
			{
				case 0:
					cfg = File.ReadAllText("config.json");
					break;
				case 1:
					cfg = File.ReadAllText(args[0]);
					break;
				case 2:
					File.WriteAllText(args[1], ConfigLoad.Save(new Config()));
					cfg = File.ReadAllText(args[0]);
					break;
				default:
					throw new ArgumentException();
			}
			var game = new MainGame.MainGame(ConfigLoad.Parse(cfg));
			game.Run();
		}*/
	    [STAThread]
	    public static void Main(string[] args)
	    {
	        var application = new Eto.Forms.Application();
	        var form = new LauncherForm();
	        application.Run(form);
	    }
	}
}
