using System;
using System.IO;
using snake_game.Launcher;

namespace Launcher
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var application = new Eto.Forms.Application();
            var form = new LauncherForm();
            try
            {
                //application.Initialized += (args, sender) => form.Show();
                //application.Run();
                application.Run(form);
            }
            catch (Exception e)
            {
                using (var writer = new StreamWriter(@"log.log", true))
                {
                    writer.WriteLine(e.Message);
                    writer.WriteLine(Environment.StackTrace);
                }
                throw;
            }
        }
    }
}