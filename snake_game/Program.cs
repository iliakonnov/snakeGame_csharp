using System;
using System.IO;
using snake_game.Launcher;
using snake_game.MainGame;

namespace snake_game
{
    public static class Program
	{
	    [STAThread]
	    public static void Main(string[] args)
	    {
	        var application = new Eto.Forms.Application();
	        var form = new LauncherForm();
	        application.Run(form);
	    }
	}
}
