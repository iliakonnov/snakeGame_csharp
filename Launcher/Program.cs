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
            #if RELEASE
            try
            {
            #endif
                var application = new Eto.Forms.Application();
                var form = new LauncherForm();
                application.Run(form);
            #if RELEASE
            }
            catch (Exception e)
            {
                using (var writer = new StreamWriter(@"log.log", true))
                {
                    writer.WriteLine(e);
                    writer.WriteLine(e.Message);
                    writer.WriteLine(Environment.StackTrace);
                }
                throw;
            }
            #endif
        }
    }
}